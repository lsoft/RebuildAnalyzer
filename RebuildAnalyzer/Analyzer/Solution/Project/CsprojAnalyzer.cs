using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Evaluation.Context;
using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.Analyzer.Result;
using RebuildAnalyzer.Helper;
using RebuildAnalyzer.MsBuild;
using System.Diagnostics;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public sealed class CsprojAnalyzer : IProjectAnalyzer
    {
        private readonly EvaluationContext _evaluationContext;

        private HashSet<string>? _projectFiles;
        private object? _additionalAnalyzerResults;

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

        public void Prepare(
            AnalyzeRequest request
            )
        {
            var projectXmlRoot = Microsoft.Build.Construction.ProjectRootElement.Open(
                CsprojFullFilePath
                );

            var (configurations, targetFrameworks) = DetermineConfigurationAndTargetFramework(
                projectXmlRoot
                );

            _projectFiles = new HashSet<string>();
            foreach (var configuration in configurations)
            {
                foreach (var targetFramework in targetFrameworks)
                {
                    var gp = new Dictionary<string, string>();
                    gp["Configuration"] = configuration;
                    gp["TargetFramework"] = targetFramework;

                    //var sw = Stopwatch.StartNew();
                    //var ev2 = sw.Elapsed;
                    using var evaluatedProject = EvaluateProject(
                        projectXmlRoot,
                        new Microsoft.Build.Definition.ProjectOptions
                        {
                            GlobalProperties = gp,
                            EvaluationContext = _evaluationContext
                        }
                        );
                    //var ev3 = sw.Elapsed;
                    //Console.WriteLine("--2> " + (ev3 - ev2) + "          " + configuration + ", " + targetFramework);

                    if (request.AdditionalProjectAnalyzer is not null)
                    {
                        _additionalAnalyzerResults = request.AdditionalProjectAnalyzer(
                            evaluatedProject.Project,
                            _additionalAnalyzerResults
                            );
                    }

                    var projectFiles = evaluatedProject.ScanForProjectFiles(
                        RootFolder,
                        CsprojFullFolderPath
                        );
                    _projectFiles.AddRange(projectFiles);
                }
            }
        }

        public AffectedSubjectPart? IsAffected(
            AnalyzeRequest request
            )
        {
            if (_projectFiles is null)
            {
                throw new InvalidOperationException("Csproj analyzer is not prepared.");
            }

            var subChangeset = request.Changeset.BuildSubChangeset(_projectFiles);

            if (request.Changeset.Contains(CsprojRelativeFilePath))
            {
                //csproj itself has changed

                if (subChangeset is not null)
                {
                    return new AffectedSubjectPart(
                        CsprojRelativeFilePath,
                        subChangeset.Append(CsprojRelativeFilePath),
                        _additionalAnalyzerResults
                        );
                }
                else
                {
                    return new AffectedSubjectPart(
                        CsprojRelativeFilePath,
                        new Changeset(CsprojRelativeFilePath),
                        _additionalAnalyzerResults
                        );
                }
            }

            if (subChangeset is null)
            {
                return null;
            }

            return new AffectedSubjectPart(
                CsprojRelativeFilePath,
                subChangeset,
                _additionalAnalyzerResults
                );
        }

        private (IReadOnlyList<string> configurations, IReadOnlyList<string> targetFrameworks) DetermineConfigurationAndTargetFramework(
            Microsoft.Build.Construction.ProjectRootElement projectXmlRoot
            )
        {
            //var sw = Stopwatch.StartNew();

            using var preEvaluatedProject = EvaluateProject(
                projectXmlRoot
                );

            //var ev1 = sw.Elapsed;
            //Console.WriteLine("--1> " + ev1);

            var configurations = preEvaluatedProject.DetermineConfigurations();
            var targetFrameworks = preEvaluatedProject.DetermineTargetFrameworks();
            return (configurations, targetFrameworks);
        }

        private EvaluationProjectWrapper EvaluateProject(
            Microsoft.Build.Construction.ProjectRootElement projectXmlRoot,
            ProjectOptions? projectOptions = null
            )
        {
            var workingProjectOptions = projectOptions ?? new Microsoft.Build.Definition.ProjectOptions
            {
                EvaluationContext = _evaluationContext
            };

            var evaluatedProject = Microsoft.Build.Evaluation.Project.FromProjectRootElement(
                projectXmlRoot,
                workingProjectOptions
                );

            return new EvaluationProjectWrapper(
                evaluatedProject
                );
        }

    }
}
