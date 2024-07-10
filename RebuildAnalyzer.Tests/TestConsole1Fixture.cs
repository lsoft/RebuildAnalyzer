using RebuildAnalyzer.Analyzer;

namespace RebuildAnalyzer.Tests
{
    public class TestConsole1Fixture
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RegularItem()
        {
            var changeset = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.Console1\\Program.cs"
                }.ToHashSet()
                );

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
        }

        [Test]
        public void ItemWith_TargetFramework_Condition()
        {
            var changeset = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.Console1\\DotNet7File.cs"
                }.ToHashSet()
                );

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
        }

        [Test]
        public void ItemWith_DebugRelease_Condition()
        {
            var changeset = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.Console1\\ReleaseFile.cs"
                }.ToHashSet()
                );

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
        }


        [Test]
        public void SharedItem()
        {
            var changeset = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.Shared12\\SharedClass.cs"
                }.ToHashSet()
                );

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);
            var ass = ar.Results.Select(a => a.Subject).ToList();

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
        }

        public sealed class RepositoryAnalyzer : Analyzer.RepositoryAnalyzer
        {
            public const string SlnRelativeFilePath = "RebuildAnalyzer.Test.Subject.Console1.sln";
            public const string Project_Console1_RelativeFilePath = @"RebuildAnalyzer.Test.Subject.Console1\RebuildAnalyzer.Test.Subject.Console1.csproj";

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
                        SlnRelativeFilePath,
                        AnalyzeSubjectKindEnum.Sln
                        ),
                };
            }
        }

    }
}