﻿using RebuildAnalyzer.Analyzer.Solution;
using RebuildAnalyzer.Analyzer.Solution.Project.Factory;

namespace RebuildAnalyzer.Analyzer
{
    /// <summary>
    /// Results of repository analyze.
    /// </summary>
    public sealed class AnalyzeResult
    {
        private readonly List<SubjectAnalyzeResult> _results = new();

        public IReadOnlyList<SubjectAnalyzeResult> Results => _results;

        internal AnalyzeResult()
        {
        }

        internal AnalyzeResult(
            List<SubjectAnalyzeResult> results
            )
        {
            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            _results = results;
        }
    }


    /// <summary>
    /// Results of subject analyze.
    /// </summary>
    public sealed class SubjectAnalyzeResult
    {
        public AnalyzeSubject Subject
        {
            get;
        }
        public IReadOnlyList<AffectedSubjectPart> AffectedParts
        {
            get;
        }

        internal SubjectAnalyzeResult(
            AnalyzeSubject subject
            ) : this(subject, new List<AffectedSubjectPart>())
        {
        }

        internal SubjectAnalyzeResult(
            AnalyzeSubject subject,
            AffectedSubjectPart affectedPart
            ) : this(subject, new List<AffectedSubjectPart> { affectedPart })
        {
        }

        internal SubjectAnalyzeResult(
            AnalyzeSubject subject,
            IReadOnlyList<AffectedSubjectPart>? affectedParts
            )
        {
            if (subject is null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (affectedParts is null)
            {
                throw new ArgumentNullException(nameof(affectedParts));
            }

            Subject = subject;
            AffectedParts = affectedParts ?? new List<AffectedSubjectPart>();
        }
    }

    /// <summary>
    /// Affected subject part (usually, a project, csproj or something).
    /// </summary>
    public sealed class AffectedSubjectPart
    {
        public AnalyzeSubjectKindEnum Kind
        {
            get;
        }
        public string RelativeProjectFilePath
        {
            get;
        }

        internal AffectedSubjectPart(
            AnalyzeSubjectKindEnum kind,
            string relativeProjectFilePath
            )
        {
            if (relativeProjectFilePath is null)
            {
                throw new ArgumentNullException(nameof(relativeProjectFilePath));
            }

            Kind = kind;
            RelativeProjectFilePath = relativeProjectFilePath;
        }
    }

    public abstract class RepositoryAnalyzer
    {
        public const string DirectoryBuildProps = "Directory.Build.props";
        public const string DirectoryPackagesProps = "Directory.Packages.props";

        protected abstract IReadOnlyList<string> GetSkippedProjects();

        protected abstract List<AnalyzeSubject> ScanForSubjects();

        public AnalyzeResult DetermineAffectedSubjects(Changeset changeset)
        {
            if(changeset.IsEmpty)
            {
                return new AnalyzeResult();
            }

            var projectAnalyzerFactory =
                new CachedProjectAnalyzerFactory(
                    new ProjectAnalyzerFactory(
                        )
                    );

            var subjects = ScanForSubjects();

            if (changeset.ContainsAtAnyLevel(DirectoryBuildProps))
            {
                //все субъекты затронуты
                return new(
                    subjects.ConvertAll(a => new SubjectAnalyzeResult(a))
                    );
            }
            if (changeset.ContainsAtAnyLevel(DirectoryPackagesProps))
            {
                //все субъекты затронуты
                return new(
                    subjects.ConvertAll(a => new SubjectAnalyzeResult(a))
                    );
            }
            //TODO: Добавить еще файлы, которые влияют на множество sln, slnf, csproj

            var subjectAnalyzeResults = new List<SubjectAnalyzeResult>();

            var skippedProjects = GetSkippedProjects();
            foreach (var subject in subjects)
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
                            if (slna.IsAffected(changeset, out var affectedParts))
                            {
                                subjectAnalyzeResults.Add(
                                    new SubjectAnalyzeResult(
                                        subject,
                                        affectedParts
                                        )
                                    );
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
                            if (slnfa.IsAffected(changeset, out var affectedParts))
                            {
                                subjectAnalyzeResults.Add(
                                    new SubjectAnalyzeResult(
                                        subject,
                                        affectedParts
                                        )
                                    );
                            }
                        }
                        break;
                    case AnalyzeSubjectKindEnum.Project:
                        {
                            if (!skippedProjects.Contains(subject.RelativeFilePath))
                            {
                                var pa = projectAnalyzerFactory.TryCreate(
                                    subject.RootFolder,
                                    subject.RelativeFilePath
                                    );
                                if (pa != null)
                                {
                                    if (pa.IsAffected(changeset))
                                    {
                                        subjectAnalyzeResults.Add(
                                            new SubjectAnalyzeResult(
                                                subject,
                                                new AffectedSubjectPart(
                                                    AnalyzeSubjectKindEnum.Project,
                                                    subject.RelativeFilePath
                                                    )
                                                )
                                            );
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException(subject.Kind.ToString());
                }
            }

            Console.WriteLine($"Cached analyzer factory stat: cached {projectAnalyzerFactory.CachedAnalyzerCreateInvocation} out of total {projectAnalyzerFactory.TotalAnalyzerCreateInvocation}, non-cached building takes {projectAnalyzerFactory.NonCachedAnalyzerCreateTimeSpent.TotalSeconds} seconds.");

            return new(
                subjectAnalyzeResults
                );
        }
    }
}
