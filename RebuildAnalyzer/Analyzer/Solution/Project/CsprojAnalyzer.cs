using Microsoft.Build.Definition;
using RebuildAnalyzer.Helper;
using RebuildAnalyzer.MsBuild;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public sealed class CsprojAnalyzer : IProjectAnalyzer
    {
        public string RootFolder { get; }
        public string CsprojRelativeFilePath { get; }

        public string CsprojFullFilePath => Path.Combine(RootFolder, CsprojRelativeFilePath);

        public string CsprojFullFolderPath => new FileInfo(CsprojFullFilePath).Directory.FullName;

        public string RelativeProjectFilePath => CsprojRelativeFilePath;

        public CsprojAnalyzer(
            string rootFolder,
            string csprojRelativeFilePath
            )
        {
            if (csprojRelativeFilePath is null)
            {
                throw new ArgumentNullException(nameof(csprojRelativeFilePath));
            }
            RootFolder = rootFolder;
            CsprojRelativeFilePath = csprojRelativeFilePath;
        }

        public bool IsAffected(Changeset changeset)
        {
            if (changeset.Contains(CsprojRelativeFilePath))
            {
                //сам csproj изменился
                return true;
            }

            var projectFiles = DetermineProjectFiles();

            var result = changeset.ContainsAny(projectFiles);

            return result;
        }

        private EvaluationProjectWrapper EvaluateProject(
            Microsoft.Build.Construction.ProjectRootElement projectXmlRoot,
            ProjectOptions? projectOptions = null
            )
        {
            var evaluatedProject = Microsoft.Build.Evaluation.Project.FromProjectRootElement(
                projectXmlRoot,
                projectOptions ?? new Microsoft.Build.Definition.ProjectOptions { }
                );

            return new EvaluationProjectWrapper(evaluatedProject);
        }

        private ICollection<string> DetermineProjectFiles()
        {
            var projectXmlRoot = Microsoft.Build.Construction.ProjectRootElement.Open(
                CsprojFullFilePath
                );

            using var preEvaluatedProject = EvaluateProject(
                projectXmlRoot
                );

            var configurations = preEvaluatedProject.DetermineConfigurations();
            var targetFrameworks = preEvaluatedProject.DetermineTargetFrameworks();

            var allProjectFiles = new HashSet<string>();
            foreach (var configuration in configurations)
            {
                foreach (var targetFramework in targetFrameworks)
                {
                    var gp = new Dictionary<string, string>();
                    gp["Configuration"] = configuration;
                    gp["TargetFramework"] = targetFramework;

                    using var evaluatedProject = EvaluateProject(
                        projectXmlRoot,
                        new Microsoft.Build.Definition.ProjectOptions
                        {
                            GlobalProperties = gp
                        }
                        );

                    var projectFiles = evaluatedProject.ScanForProjectFiles(
                        RootFolder,
                        CsprojFullFolderPath
                        );
                    allProjectFiles.AddRange(projectFiles);
                }
            }

            return allProjectFiles;
        }

    }
}
