using Microsoft.Build.Locator;
using System.Diagnostics;
using RebuildAnalyzer.Analyzer;

namespace RebuildAnalyzer.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();

            var changeset = TestRepositoryAnalyzer.TestChangeset1;

            RebuildAnalyzer.Helper.ParallelOption.MaxDegreeOfParallelism = 1;

            var sw = Stopwatch.StartNew();

            var tra = new TestRepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);
            var affs = ar.Results.Select(a => a.Subject).ToList();
            Console.WriteLine("Changed: " + string.Join(",", affs.Select(a => a.RelativeFilePath)));

            Console.WriteLine("Taken: " + sw.Elapsed);
        }

        /// <summary>
        /// Test this repository.
        /// </summary>
        public sealed class TestRepositoryAnalyzer : RepositoryAnalyzer
        {
            /// <summary>
            /// Test against multitarget csproj RebuildAnalyzer.Test.Subject.Console1
            /// </summary>
            public static Changeset TestChangeset1 = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.Console1\\DotNet7File.cs"
                }.ToHashSet()
                );

            protected override IReadOnlyList<string> GetSkippedProjects()
            {
                return new List<string>
                {
                };
            }

            protected override List<AnalyzeSubject> ScanForSubjects()
            {
                return new List<AnalyzeSubject>
                {
                    new AnalyzeSubject(
                        Path.GetFullPath(@"..\\..\\..\\..\\"),
                        @"RebuildAnalyzer.Test.Subject.Console1.sln",
                        AnalyzeSubjectKindEnum.Sln
                        ),
                    new AnalyzeSubject(
                        Path.GetFullPath(@"..\\..\\..\\..\\"),
                        @"RebuildAnalyzer.Test.Subject.Console1.sln",
                        AnalyzeSubjectKindEnum.Sln
                        ),
                    new AnalyzeSubject(
                        Path.GetFullPath(@"..\\..\\..\\..\\"),
                        @"RebuildAnalyzer.Test.Subject.Console1.sln",
                        AnalyzeSubjectKindEnum.Sln
                        ),
                    new AnalyzeSubject(
                        Path.GetFullPath(@"..\\..\\..\\..\\"),
                        @"RebuildAnalyzer.Test.Subject.Console1.sln",
                        AnalyzeSubjectKindEnum.Sln
                        ),
                    new AnalyzeSubject(
                        Path.GetFullPath(@"..\\..\\..\\..\\"),
                        @"RebuildAnalyzer.Test.Subject.Console1.sln",
                        AnalyzeSubjectKindEnum.Sln
                        ),
                    new AnalyzeSubject(
                        Path.GetFullPath(@"..\\..\\..\\..\\"),
                        @"RebuildAnalyzer.Test.Subject.Console1.sln",
                        AnalyzeSubjectKindEnum.Sln
                        ),
                };
            }
        }

    }
}
