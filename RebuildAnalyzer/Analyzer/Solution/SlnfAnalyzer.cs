using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text.Json;
using RebuildAnalyzer.Analyzer.Solution.Project.Factory;
using RebuildAnalyzer.FileStructure;

namespace RebuildAnalyzer.Analyzer.Solution
{
    public sealed class SlnfAnalyzer
    {
        private readonly IProjectAnalyzerFactory _projectAnalyzerFactory;

        public IReadOnlyList<string> SkippedProjects { get; }
        public string RootFolder { get; }
        public string SlnfRelativeFilePath { get; }

        public string SlnfFullFilePath => Path.Combine(RootFolder, SlnfRelativeFilePath);

        public SlnfAnalyzer(
            IProjectAnalyzerFactory projectAnalyzerFactory,
            IReadOnlyList<string> skippedProjects,
            string rootFolder,
            string slnfRelativeFilePath
            )
        {
            if (projectAnalyzerFactory is null)
            {
                throw new ArgumentNullException(nameof(projectAnalyzerFactory));
            }

            if (skippedProjects is null)
            {
                throw new ArgumentNullException(nameof(skippedProjects));
            }

            if (rootFolder is null)
            {
                throw new ArgumentNullException(nameof(rootFolder));
            }

            if (slnfRelativeFilePath is null)
            {
                throw new ArgumentNullException(nameof(slnfRelativeFilePath));
            }
            _projectAnalyzerFactory = projectAnalyzerFactory;
            SkippedProjects = skippedProjects;
            RootFolder = rootFolder;
            SlnfRelativeFilePath = slnfRelativeFilePath;
        }

        public bool IsAffected(
            Changeset changeset,
            out List<AffectedSubjectPart>? affectedParts
            )
        {
            if (changeset.Contains(SlnfRelativeFilePath))
            {
                //сам slnf изменился
                affectedParts = new List<AffectedSubjectPart>();
                return true;
            }

            var slnf = CreateSlnf();
            if (slnf is null)
            {
                //slnf не найден
                affectedParts = null;
                return false;
            }

            //TODO: здесь может быть ошибка с путями, если sln или slnf лежат не в корне репозитория
            //вероятно, путь к sln внутри slnf идет относительно САМОГО ФАЙЛА slnf, а не корня репозитория
            //а в ченжсете пути хранятся относительно корня репозитория
            if (changeset.Contains(slnf.solution.path))
            {
                //сам sln изменился
                affectedParts = new List<AffectedSubjectPart>();
                return true;
            }

            var slnfFolder = new FileInfo(SlnfFullFilePath).Directory.FullName;
            var slnFilePath = Path.Combine(
                slnfFolder,
                slnf.solution.path
                );
            slnFilePath = Path.GetFullPath(slnFilePath);
            var slnFolderPath = new FileInfo(slnFilePath).Directory.FullName;

            //параллелизуем для ускорения работы
            var inProcessAffectedParts = new ConcurrentBag<AffectedSubjectPart>();
            Parallel.ForEach(slnf.solution.projects, new ParallelOptions { MaxDegreeOfParallelism = RebuildAnalyzer.Helper.ParallelOption.MaxDegreeOfParallelism }, (relativeProjectFilePath, state) =>
            {
                if (SkippedProjects.Contains(relativeProjectFilePath))
                {
                    //этот проект проверять не надо
                    return;
                }

                var projectAnalyzer = _projectAnalyzerFactory.TryCreate(
                    slnFolderPath, //relativeProjectFilePath relative against SLN file, not a slnF file!
                    relativeProjectFilePath
                    );
                if (projectAnalyzer is null)
                {
                    //неизвестный проект, поэтому, на всякий случай, сообщаем, что slnf изменился
                    inProcessAffectedParts.Add(
                        new AffectedSubjectPart(
                            AnalyzeSubjectKindEnum.Project,
                            relativeProjectFilePath
                            )
                        );
                    return;
                }

                if (projectAnalyzer.IsAffected(changeset))
                {
                    //проект изменился, дальше крутить смысла нет
                    inProcessAffectedParts.Add(
                        new AffectedSubjectPart(
                            AnalyzeSubjectKindEnum.Project,
                            relativeProjectFilePath
                            )
                        );
                    return;
                }
            });

            affectedParts = inProcessAffectedParts.ToList();
            return affectedParts.Count > 0;
        }


        private SlnfJson? CreateSlnf()
        {
            var slnf = JsonSerializer.Deserialize<SlnfJson>(File.ReadAllText(SlnfFullFilePath));
            if (slnf is null)
            {
                //slnf не найден
                return null;
            }

            slnf.UpdateSlashesInPaths();

            return slnf;
        }

    }
}
