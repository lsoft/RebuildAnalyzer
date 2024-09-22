using Microsoft.Build.Evaluation;
using RebuildAnalyzer.Analyzer.Result;

namespace RebuildAnalyzer.Analyzer.Request
{
    /// <summary>
    /// Analyze request.
    /// </summary>
    public sealed class AnalyzeRequest
    {
        /// <summary>
        /// Changeset (list of changed (created, modified, deleted) files.
        /// </summary>
        public Changeset Changeset
        {
            get;
        }

        /// <summary>
        /// Additional "analyzer" for msbuild project.
        /// Its results will be included in <see cref="AffectedSubjectPart"/> inside <see cref="AnalyzeResult"/>.
        /// </summary>
        public Func<Project, object?, object?>? AdditionalProjectAnalyzer
        {
            get;
        }

        public AnalyzeRequest(
            Changeset changeset,
            Func<Project, object?, object?>? additionalProjectAnalyzer
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
}
