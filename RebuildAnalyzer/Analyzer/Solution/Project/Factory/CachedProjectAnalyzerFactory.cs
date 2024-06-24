using System.Collections.Concurrent;
using System.Diagnostics;

namespace RebuildAnalyzer.Analyzer.Solution.Project.Factory
{
    public sealed class CachedProjectAnalyzerFactory : IProjectAnalyzerFactory
    {
        private readonly IProjectAnalyzerFactory _projectAnalyzerFactory;

        private readonly ConcurrentDictionary<string, IProjectAnalyzer?> _cache = new();

        private int _totalAnalyzerCreateInvocation;
        private int _cachedAnalyzerCreatedInvocation;
        private long _nonCachedAnalyzerCreateTimeSpent;

        public int TotalAnalyzerCreateInvocation => _totalAnalyzerCreateInvocation;

        public int CachedAnalyzerCreateInvocation => _cachedAnalyzerCreatedInvocation;

        public TimeSpan NonCachedAnalyzerCreateTimeSpent => TimeSpan.FromTicks(_nonCachedAnalyzerCreateTimeSpent);

        public CachedProjectAnalyzerFactory(
            IProjectAnalyzerFactory projectAnalyzerFactory
            )
        {
            if (projectAnalyzerFactory is null)
            {
                throw new ArgumentNullException(nameof(projectAnalyzerFactory));
            }

            _projectAnalyzerFactory = projectAnalyzerFactory;
        }

        public IProjectAnalyzer? TryCreate(
            string rootFolder,
            string projectRelativeFilePath
            )
        {
            Interlocked.Increment(ref _totalAnalyzerCreateInvocation);

            //nulls inside this cache is fine
            if (!_cache.TryGetValue(projectRelativeFilePath, out var projectAnalyzer))
            {
                projectAnalyzer = CreateInnerProjectAnalyzer(
                    rootFolder,
                    projectRelativeFilePath
                    );

                _ = _cache.TryAdd(projectRelativeFilePath, projectAnalyzer);
            }
            else
            {
                Interlocked.Increment(ref _cachedAnalyzerCreatedInvocation);
            }

            return projectAnalyzer;
        }

        private IProjectAnalyzer? CreateInnerProjectAnalyzer(
            string rootFolder,
            string projectRelativeFilePath
            )
        {
            IProjectAnalyzer? result = null;

            var sw = Stopwatch.StartNew();
            try
            {
                result = _projectAnalyzerFactory.TryCreate(rootFolder, projectRelativeFilePath);
            }
            finally
            {
                var elapsed = sw.Elapsed;

                Interlocked.Add(ref _nonCachedAnalyzerCreateTimeSpent, elapsed.Ticks);

                if (result is not null)
                {
                    Console.WriteLine($"{elapsed} spent in {result.ProjectRelativeFilePath}");
                }
                else
                {
                    Console.WriteLine($"{elapsed} spent in <null> analyzer");
                }
            }

            return result;
        }
    }
}
