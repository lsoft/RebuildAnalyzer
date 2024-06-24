namespace RebuildAnalyzer.Analyzer.Solution.Project.Factory
{
    public interface IProjectAnalyzerFactory
    {
        IProjectAnalyzer? TryCreate(
            string rootFolder,
            string projectRelativeFilePath
            );
    }
}
