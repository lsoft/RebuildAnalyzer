using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.Analyzer.Result;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public interface IProjectAnalyzer
    {
        string ProjectRelativeFilePath { get; }

        void Prepare(
            AnalyzeRequest request
            );

        AffectedSubjectPart? IsAffected(
            AnalyzeRequest request
            );
    }
}
