using Microsoft.Build.Evaluation.Context;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public class ProjectAnalyzerFactory
    {
        private Microsoft.Build.Evaluation.Context.EvaluationContext _evaluationContext =
            Microsoft.Build.Evaluation.Context.EvaluationContext.Create(
                Microsoft.Build.Evaluation.Context.EvaluationContext.SharingPolicy.Shared
                );

        public IProjectAnalyzer? TryCreate(
            string rootFolder,
            string projectRelativeFilePath
            )
        {
            var realProjectAnalyzer = BuildRealAnalyzer(
                rootFolder,
                projectRelativeFilePath
                );

            return new TelemetryProjectAnalyzer(realProjectAnalyzer);
        }

        private IProjectAnalyzer BuildRealAnalyzer(
            string rootFolder,
            string projectRelativeFilePath
            )
        {
            if (projectRelativeFilePath.EndsWith(".csproj"))
            {
                return new CsprojAnalyzer(_evaluationContext, rootFolder, projectRelativeFilePath);
            }
            if (projectRelativeFilePath.EndsWith(".shproj"))
            {
                return new ShprojAnalyzer(rootFolder, projectRelativeFilePath);
            }

            throw new NotImplementedException(projectRelativeFilePath);
        }
    }
}
