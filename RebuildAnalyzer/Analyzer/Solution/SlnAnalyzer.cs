using Microsoft.Build.Construction;
using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.Analyzer.Result;
using RebuildAnalyzer.Analyzer.Solution.Project.Factory;
using RebuildAnalyzer.Helper;
using System.Collections.Concurrent;

namespace RebuildAnalyzer.Analyzer.Solution
{
    public sealed class SlnAnalyzer
    {
        private readonly IProjectAnalyzerFactory _projectAnalyzerFactory;

        public IReadOnlyList<string> SkippedProjects { get; }
        public string RootFolder { get; }
        public string SlnRelativeFilePath { get; }

        public string SlnFullFilePath => Path.Combine(RootFolder, SlnRelativeFilePath);

        public SlnAnalyzer(
            IProjectAnalyzerFactory projectAnalyzerFactory,
            IReadOnlyList<string> skippedProjects,
            string rootFolder,
            string slnRelativeFilePath
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

            if (slnRelativeFilePath is null)
            {
                throw new ArgumentNullException(nameof(slnRelativeFilePath));
            }

            _projectAnalyzerFactory = projectAnalyzerFactory;
            SkippedProjects = skippedProjects;
            RootFolder = rootFolder;
            SlnRelativeFilePath = slnRelativeFilePath;
        }

        public bool IsAffected(
            AnalyzeRequest request,
            out List<AffectedSubjectPart>? affectedParts
            )
        {
            var sln = Microsoft.Build.Construction.SolutionFile.Parse(SlnFullFilePath);
            if (sln is null)
            {
                //sln не найден
                affectedParts = null;
                return false;
            }

            var slnFolder = new FileInfo(SlnFullFilePath).Directory.FullName;

            //параллелизуем для ускорения работы
            var inProcessAffectedParts = new ConcurrentBag<AffectedSubjectPart>();
            Parallel.ForEach(sln.ProjectsInOrder, new ParallelOptions { MaxDegreeOfParallelism = RebuildAnalyzer.Helper.ParallelOption.MaxDegreeOfParallelism }, (projectInSolution, state) =>
            {
                if(projectInSolution.ProjectType.In(SolutionProjectType.SolutionFolder))
                {
                    return;
                }

                var relativeProjectFilePath = projectInSolution.RelativePath;

                if (SkippedProjects.Contains(relativeProjectFilePath))
                {
                    //этот проект проверять не надо
                    return;
                }

                var projectAnalyzer = _projectAnalyzerFactory.TryCreate(
                    slnFolder,
                    relativeProjectFilePath,
                    request
                    );
                if (projectAnalyzer is null)
                {
                    //неизвестный проект, поэтому, на всякий случай, сообщаем, что sln изменился
                    inProcessAffectedParts.Add(
                        new AffectedSubjectPart(
                            relativeProjectFilePath,
                            new Changeset(relativeProjectFilePath),
                            null
                            )
                        );
                    return;
                }

                var affectedSubjectPart = projectAnalyzer.IsAffected(request);
                if (affectedSubjectPart is not null)
                {
                    //if (!affectedSubjectPart.Changeset.IsEmpty)
                    {
                        //файлы в проекте изменились
                        inProcessAffectedParts.Add(affectedSubjectPart);
                        return;
                    }
                }
            });

            affectedParts = inProcessAffectedParts.ToList();

            //after we determined affected parts, and REGARDLESS of these affected parts
            //we need to check if sln files has changed and return true if so.
            //we cannot do this before parts processing because we need to return actual
            //affected parts even if sln has changed
            //(merge request can change cs files and sln files and we must return
            //affected parts regardless of sln status).

            //check if sln has changed
            if (request.Changeset.Contains(SlnRelativeFilePath))
            {
                //сам sln изменился
                return true;
            }

            return affectedParts.Count > 0;
        }

    }
}
