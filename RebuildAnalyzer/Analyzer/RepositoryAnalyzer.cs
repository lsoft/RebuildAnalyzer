﻿using Microsoft.Build.Evaluation;
using RebuildAnalyzer.Analyzer.Solution;
using RebuildAnalyzer.Analyzer.Solution.Project.Factory;
using System.Runtime;

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
        public string RelativeProjectFilePath
        {
            get;
        }

        public Changeset Changeset
        {
            get;
        }

        public object? AdditionalAnalyzerResults
        {
            get;
        }

        internal AffectedSubjectPart(
            string relativeProjectFilePath,
            Changeset changeset,
            object? additionalAnalyzerResults
            )
        {
            if (string.IsNullOrEmpty(relativeProjectFilePath))
            {
                throw new ArgumentException($"'{nameof(relativeProjectFilePath)}' cannot be null or empty.", nameof(relativeProjectFilePath));
            }

            if (changeset is null)
            {
                throw new ArgumentNullException(nameof(changeset));
            }

            RelativeProjectFilePath = relativeProjectFilePath;
            Changeset = changeset;
            AdditionalAnalyzerResults = additionalAnalyzerResults;
        }
    }

    /// <summary>
    /// Analyze request.
    /// </summary>
    public sealed class AnalyzeRequest
    {
        /// <summary>
        /// Changeset (list of changed (created, modified, deleted) files.
        /// </summary>
        public Changeset Changeset { get; }

        /// <summary>
        /// Additional "analyzer" for msbuild project.
        /// Its results will be included in <see cref="AffectedSubjectPart"/> inside <see cref="AnalyzeResult"/>.
        /// </summary>
        public Func<Project, object?, object?>? AdditionalProjectAnalyzer { get; }

        public AnalyzeRequest(
            Changeset changeset,
            Func<Microsoft.Build.Evaluation.Project, object?, object?>? additionalProjectAnalyzer
            )
        {
            if (changeset is null)
            {
                throw new ArgumentNullException(nameof(changeset));
            }

            Changeset = changeset;
            AdditionalProjectAnalyzer = additionalProjectAnalyzer;
        }

    }


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
                                subjectAnalyzeResults.Add(
                                    new SubjectAnalyzeResult(
                                        subject,
                                        affectedParts
                                        )
                                    );
                            }
                            else if(everythingChanged)
                            {
                                subjectAnalyzeResults.Add(
                                    new SubjectAnalyzeResult(subject)
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
                            if (slnfa.IsAffected(request, out var affectedParts))
                            {
                                subjectAnalyzeResults.Add(
                                    new SubjectAnalyzeResult(
                                        subject,
                                        affectedParts
                                        )
                                    );
                            }
                            else if (everythingChanged)
                            {
                                subjectAnalyzeResults.Add(
                                    new SubjectAnalyzeResult(subject)
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
                                    subject.RelativeFilePath,
                                    request
                                    );
                                var affectedSubjectPart = pa?.IsAffected(request);
                                if (affectedSubjectPart is not null)
                                {
                                    subjectAnalyzeResults.Add(
                                        new SubjectAnalyzeResult(
                                            subject,
                                            affectedSubjectPart
                                            )
                                        );
                                }
                                else if (everythingChanged)
                                {
                                    subjectAnalyzeResults.Add(
                                        new SubjectAnalyzeResult(subject)
                                        );
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
