namespace RebuildAnalyzer.Analyzer.Result
{
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
}
