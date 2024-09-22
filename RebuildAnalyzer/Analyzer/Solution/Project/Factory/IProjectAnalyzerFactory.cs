using RebuildAnalyzer.Analyzer.Request;

namespace RebuildAnalyzer.Analyzer.Solution.Project.Factory
{
    public interface IProjectAnalyzerFactory
    {
        IProjectAnalyzer? TryCreate(
            string rootFolder,
            string projectRelativeFilePath,
            AnalyzeRequest request
            );
    }
}
