namespace RebuildAnalyzer.Analyzer.Result
{
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
}
