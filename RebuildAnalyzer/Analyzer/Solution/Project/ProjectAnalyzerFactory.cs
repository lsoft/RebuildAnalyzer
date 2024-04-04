namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public static class ProjectAnalyzerFactory
    {
        public static IProjectAnalyzer? TryCreate(
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

        public static IProjectAnalyzer BuildRealAnalyzer(
            string rootFolder,
            string projectRelativeFilePath
            )
        {
            if (projectRelativeFilePath.EndsWith(".csproj"))
            {
                return new CsprojAnalyzer(rootFolder, projectRelativeFilePath);
            }
            if (projectRelativeFilePath.EndsWith(".shproj"))
            {
                return new ShprojAnalyzer(rootFolder, projectRelativeFilePath);
            }

            throw new NotImplementedException(projectRelativeFilePath);
        }
    }
}
