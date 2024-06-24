using Microsoft.Build.Evaluation.Context;
using System.Runtime.CompilerServices;

namespace RebuildAnalyzer.Analyzer.Solution.Project.Factory
{
    public sealed class ProjectAnalyzerFactory : IProjectAnalyzerFactory
    {
        private EvaluationContext _evaluationContext =
            EvaluationContext.Create(
                EvaluationContext.SharingPolicy.Shared
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

            realProjectAnalyzer.Prepare();

            return realProjectAnalyzer;
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
