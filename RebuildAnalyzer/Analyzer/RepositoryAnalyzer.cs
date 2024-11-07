using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.Analyzer.Result;
using RebuildAnalyzer.Analyzer.Solution;
using RebuildAnalyzer.Analyzer.Solution.Project.Factory;
using System.Runtime;

namespace RebuildAnalyzer.Analyzer
{
    public abstract class RepositoryAnalyzer
    {
        public const string DirectoryBuildProps = "Directory.Build.props";
        public const string DirectoryPackagesProps = "Directory.Packages.props";

        protected abstract IReadOnlyList<string> GetSkippedProjects();

        protected abstract List<AnalyzeSubject> ScanForSubjects();

        public AnalyzeResult DetermineAffectedSubjects(
            AnalyzeRequest request
            )
        {
            if(request.Changeset.IsEmpty)
            {
                return new AnalyzeResult();
            }

            var projectAnalyzerFactory =
                new CachedProjectAnalyzerFactory(
                    new ProjectAnalyzerFactory(
                        )
                    );

            var subjects = ScanForSubjects();
            var skippedProjects = GetSkippedProjects();

            var everythingChanged = false;
            if (request.Changeset.ContainsAtAnyLevel(DirectoryBuildProps))
            {
                //we threat this as every subject has changed
                everythingChanged = true;
            }
            if (request.Changeset.ContainsAtAnyLevel(DirectoryPackagesProps))
            {
                //we threat this as every subject has changed
                everythingChanged = true;
            }
            //TODO: Добавить еще файлы, которые влияют на множество sln, slnf, csproj


            var subjectAnalyzeResults = new List<SubjectAnalyzeResult>();
            foreach (var subject in subjects)
            {
                try
                {
                    var subjectAnalyzeResult = ProcessSubject(
                        request,
                        subject,
                        projectAnalyzerFactory,
                        skippedProjects,
                        everythingChanged
                        );
                    if (subjectAnalyzeResult is not null)
                    {
                        subjectAnalyzeResults.Add(subjectAnalyzeResult);
                    }
                }
                catch (Exception ex)
                {
                    throw new RepositoryAnalyzeException(
                        $"Error has been detected when processing {subject.FullFilePath}",
                        ex
                        );
                }
            }

            Console.WriteLine($"Cached analyzer factory stat: cached {projectAnalyzerFactory.CachedAnalyzerCreateInvocation} out of total {projectAnalyzerFactory.TotalAnalyzerCreateInvocation}, non-cached building takes {projectAnalyzerFactory.NonCachedAnalyzerCreateTimeSpent.TotalSeconds} seconds.");

            return new(
                subjectAnalyzeResults
                );
        }

        private static SubjectAnalyzeResult? ProcessSubject(
            AnalyzeRequest request,
            AnalyzeSubject subject,
            CachedProjectAnalyzerFactory projectAnalyzerFactory,
            IReadOnlyList<string> skippedProjects,
            bool everythingChanged
            )
        {
            switch (subject.Kind)
            {
                case AnalyzeSubjectKindEnum.Sln:
                    {
                        var slna = new SlnAnalyzer(
                            projectAnalyzerFactory,
                            skippedProjects,
                            subject.RootFolder,
                            subject.RelativeFilePath
                            );
                        if (slna.IsAffected(request, out var affectedParts))
                        {
                            return
                                new SubjectAnalyzeResult(
                                    subject,
                                    affectedParts
                                    );
                        }
                        else if (everythingChanged)
                        {
                            return
                                new SubjectAnalyzeResult(subject);
                        }
                    }
                    break;
                case AnalyzeSubjectKindEnum.Slnf:
                    {
                        var slnfa = new SlnfAnalyzer(
                            projectAnalyzerFactory,
                            skippedProjects,
                            subject.RootFolder,
                            subject.RelativeFilePath
                            );
                        if (slnfa.IsAffected(request, out var affectedParts))
                        {
                            return
                                new SubjectAnalyzeResult(
                                    subject,
                                    affectedParts
                                    );
                        }
                        else if (everythingChanged)
                        {
                            return
                                new SubjectAnalyzeResult(subject);
                        }
                    }
                    break;
                case AnalyzeSubjectKindEnum.Project:
                    {
                        if (skippedProjects.Contains(subject.RelativeFilePath))
                        {
                            return null;
                        }

                        var pa = projectAnalyzerFactory.TryCreate(
                            subject.RootFolder,
                            subject.RelativeFilePath,
                            request
                            );
                        var affectedSubjectPart = pa?.IsAffected(request);
                        if (affectedSubjectPart is not null)
                        {
                            return
                                new SubjectAnalyzeResult(
                                subject,
                                affectedSubjectPart
                                );
                        }
                        else if (everythingChanged)
                        {
                            return
                                new SubjectAnalyzeResult(subject);
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException($"{subject.Kind} is unknown kind of subject.");
            }

            return null;
        }
    }
}
