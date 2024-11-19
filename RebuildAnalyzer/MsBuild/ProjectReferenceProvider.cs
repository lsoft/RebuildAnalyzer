using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Evaluation.Context;

namespace RebuildAnalyzer.MsBuild
{
    public sealed class ProjectReferenceProvider
    {
        private readonly EvaluationContext? _evaluationContext;
        private readonly ProjectCollection? _projectCollection;
        private readonly Action<EvaluationProjectWrapper> _externalEvaluator;

        public ProjectReferenceProvider(
            EvaluationContext? evaluationContext,
            Microsoft.Build.Evaluation.ProjectCollection? projectCollection,
            Action<EvaluationProjectWrapper> externalEvaluator
            )
        {
            if (externalEvaluator is null)
            {
                throw new ArgumentNullException(nameof(externalEvaluator));
            }

            _evaluationContext = evaluationContext;
            _projectCollection = projectCollection;
            _externalEvaluator = externalEvaluator;
        }

        public void EnumerateProjectReferences(
            Microsoft.Build.Construction.ProjectRootElement projectXmlRoot
            )
        {
            var (configurations, targetFrameworks) = DetermineConfigurationAndTargetFramework(
                projectXmlRoot
                );

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
                        new ProjectOptions
                        {
                            GlobalProperties = gp,
                            EvaluationContext = _evaluationContext,
                            ProjectCollection = _projectCollection
                        }
                        );

                    //var ev3 = sw.Elapsed;
                    //Console.WriteLine("--2> " + (ev3 - ev2) + "          " + configuration + ", " + targetFramework);

                    _externalEvaluator(evaluatedProject);
                    
                    //evaluatedProject.Dispose();
                    //_projectCollection?.UnloadProject(evaluatedProject);
                }
            }
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
            var workingProjectOptions = projectOptions ?? new ProjectOptions
            {
                EvaluationContext = _evaluationContext
            };

            return new EvaluationProjectWrapper(
                projectXmlRoot,
                workingProjectOptions
                );
        }

    }
}
