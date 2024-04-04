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

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
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

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
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

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
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

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
        }

        public sealed class RepositoryAnalyzer : Analyzer.RepositoryAnalyzer
        {
            public const string SlnRelativeFilePath = "RebuildAnalyzer.Test.Subject.Console1.sln";

            protected override IReadOnlyList<string> GetSkippedProjects()
            {
                return new List<string>
                {
                };
            }

            protected override IReadOnlyList<AnalyzeSubject> ScanForSubjects()
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