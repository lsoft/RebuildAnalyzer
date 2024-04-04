using System.Text.Json;
using RebuildAnalyzer.Analyzer.Solution.Project;
using RebuildAnalyzer.FileStructure;

namespace RebuildAnalyzer.Analyzer.Solution
{
    public sealed class SlnfAnalyzer
    {
        public IReadOnlyList<string> SkippedProjects { get; }
        public string RootFolder { get; }
        public string SlnfRelativeFilePath { get; }

        public string SlnfFullFilePath => Path.Combine(RootFolder, SlnfRelativeFilePath);

        public SlnfAnalyzer(
            IReadOnlyList<string> skippedProjects,
            string rootFolder,
            string slnfRelativeFilePath
            )
        {
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
            SkippedProjects = skippedProjects;
            RootFolder = rootFolder;
            SlnfRelativeFilePath = slnfRelativeFilePath;
        }

        public bool IsAffected(Changeset changeset)
        {
            if (changeset.Contains(SlnfRelativeFilePath))
            {
                //сам slnf изменился
                return true;
            }

            var slnf = JsonSerializer.Deserialize<SlnfJson>(File.ReadAllText(SlnfFullFilePath));

            if (slnf is null)
            {
                //slnf не найден
                return false;
            }

            //TODO: здесь может быть ошибка с путями, если sln или slnf лежат не в корне репозитория
            //вероятно, путь к sln внутри slnf идет относительно САМОГО ФАЙЛА slnf, а не корня репозитория
            //а в ченжсете пути хранятся относительно корня репозитория
            if (changeset.Contains(slnf.solution.path))
            {
                //сам sln изменился
                return true;
            }

            var slnfFolder = new FileInfo(SlnfFullFilePath).Directory.FullName;
            var slnFilePath = Path.Combine(
                slnfFolder,
                slnf.solution.path
                );
            slnFilePath = Path.GetFullPath(slnFilePath);
            var slnFolderPath = new FileInfo(slnFilePath).Directory.FullName;

            var result = false;

            //параллелизуем для ускорения работы
            Parallel.ForEach(slnf.solution.projects, new ParallelOptions { MaxDegreeOfParallelism = RebuildAnalyzer.Helper.ParallelOption.MaxDegreeOfParallelism }, (relativeProjectFilePath, state) =>
            {
                if (SkippedProjects.Contains(relativeProjectFilePath))
                {
                    //этот проект проверять не надо
                    return;
                }

                var projectAnalyzer = ProjectAnalyzerFactory.TryCreate(
                    slnFolderPath, //relativeProjectFilePath relative against SLN file, not a slnF file!
                    relativeProjectFilePath
                    );
                if (projectAnalyzer is null)
                {
                    //неизвестный проект, поэтому, на всякий случай, сообщаем, что slnf изменился
                    result = true;
                    state.Break();
                    return;
                }

                if (projectAnalyzer.IsAffected(changeset))
                {
                    //проект изменился, дальше крутить смысла нет
                    result = true;
                    state.Break();
                    return;
                }
            });

            return result;
        }

    }
}
