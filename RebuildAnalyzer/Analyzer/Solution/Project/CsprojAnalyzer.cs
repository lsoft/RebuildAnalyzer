using Microsoft.Build.Evaluation.Context;
using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.Analyzer.Result;
using RebuildAnalyzer.Helper;
using RebuildAnalyzer.MsBuild;

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
            _projectFiles = new HashSet<string>();

            var evaluator = new ProjectReferenceProvider(
                _evaluationContext,
                null,
                evaluatedProject =>
                {
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
                );

            var projectXmlRoot = Microsoft.Build.Construction.ProjectRootElement.Open(
                CsprojFullFilePath
                );

            evaluator.EnumerateProjectReferences(
                projectXmlRoot
                );
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
    }
}
