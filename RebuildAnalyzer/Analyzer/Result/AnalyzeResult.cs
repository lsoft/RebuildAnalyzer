namespace RebuildAnalyzer.Analyzer.Result
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
}
