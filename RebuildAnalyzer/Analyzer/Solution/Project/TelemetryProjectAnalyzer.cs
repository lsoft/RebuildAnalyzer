using System.Diagnostics;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public sealed class TelemetryProjectAnalyzer : IProjectAnalyzer
    {
        private readonly IProjectAnalyzer _projectAnalyzer;

        public string RelativeProjectFilePath => _projectAnalyzer.RelativeProjectFilePath;

        public TelemetryProjectAnalyzer(
            IProjectAnalyzer projectAnalyzer
            )
        {
            if (projectAnalyzer is null)
            {
                throw new ArgumentNullException(nameof(projectAnalyzer));
            }

            _projectAnalyzer = projectAnalyzer;
        }

        public bool IsAffected(Changeset changeset)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return _projectAnalyzer.IsAffected(changeset);
            }
            finally
            {
                Console.WriteLine($"{sw.Elapsed} spent in {_projectAnalyzer.RelativeProjectFilePath}");
            }

        }
    }
}
