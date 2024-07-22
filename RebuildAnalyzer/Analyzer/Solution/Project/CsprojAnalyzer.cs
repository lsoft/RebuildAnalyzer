using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation.Context;
using RebuildAnalyzer.Helper;
using RebuildAnalyzer.MsBuild;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public sealed class CsprojAnalyzer : IProjectAnalyzer
    {
        private readonly EvaluationContext _evaluationContext;

        private HashSet<string>? _projectFiles;

        public string RootFolder { get; }

        public string CsprojRelativeFilePath { get; }

        public string CsprojFullFilePath => Path.Combine(RootFolder, CsprojRelativeFilePath);

        public string CsprojFullFolderPath => new FileInfo(CsprojFullFilePath).Directory.FullName;

        public string ProjectRelativeFilePath => CsprojRelativeFilePath;


        public CsprojAnalyzer(
            Microsoft.Build.Evaluation.Context.EvaluationContext evaluationContext,
            string rootFolder,
            string csprojRelativeFilePath
            )
        {
            if (evaluationContext is null)
            {
                throw new ArgumentNullException(nameof(evaluationContext));
            }

            if (rootFolder is null)
            {
                throw new ArgumentNullException(nameof(rootFolder));
            }

            if (csprojRelativeFilePath is null)
            {
                throw new ArgumentNullException(nameof(csprojRelativeFilePath));
            }

            _evaluationContext = evaluationContext;
            RootFolder = rootFolder;
            CsprojRelativeFilePath = csprojRelativeFilePath;
        }

        public void Prepare()
        {
            _projectFiles = DetermineProjectFiles();
        }


        public Changeset? IsAffected(Changeset changeset)
        {
            if (_projectFiles is null)
            {
                throw new InvalidOperationException("Csproj analyzer is not prepared.");
            }

            var subChangeset = changeset.BuildSubChangeset(_projectFiles);

            if (changeset.Contains(CsprojRelativeFilePath))
            {
                //csproj itself has changed

                if (subChangeset is not null)
                {
                    return subChangeset.Append(CsprojRelativeFilePath);
                }
                else
                {
                    return new Changeset(CsprojRelativeFilePath);
                }
            }

            return subChangeset;
        }

        private EvaluationProjectWrapper EvaluateProject(
            Microsoft.Build.Construction.ProjectRootElement projectXmlRoot,
            ProjectOptions? projectOptions = null
            )
        {
            var evaluatedProject = Microsoft.Build.Evaluation.Project.FromProjectRootElement(
                projectXmlRoot,
                projectOptions ?? new Microsoft.Build.Definition.ProjectOptions
                {
                    EvaluationContext = _evaluationContext
                }
                );

            return new EvaluationProjectWrapper(evaluatedProject);
        }


        private HashSet<string> DetermineProjectFiles()
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
                            GlobalProperties = gp,
                            EvaluationContext = _evaluationContext
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
