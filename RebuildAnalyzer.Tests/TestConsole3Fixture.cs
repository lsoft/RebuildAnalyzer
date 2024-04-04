using RebuildAnalyzer.Analyzer;

namespace RebuildAnalyzer.Tests
{
    public class TestConsole3Fixture
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
                    "RebuildAnalyzer.Test.Subject.Console3\\Program.cs"
                }.ToHashSet()
                );

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
        }


        [Test]
        public void CommonItem()
        {
            var changeset = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.Common23\\CommonClass.cs"
                }.ToHashSet()
                );

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
        }

        [Test]
        public void ReferencedDll()
        {
            var changeset = new Changeset(
                new[]
                {
                    "RebuildAnalyzer.Test.Subject.RandomStuff\\fake_stuff.dll"
                }.ToHashSet()
                );

            var ra = new RepositoryAnalyzer();
            var ass = ra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
        }

        public sealed class RepositoryAnalyzer : Analyzer.RepositoryAnalyzer
        {
            public const string SlnRelativeFilePath = "RebuildAnalyzer.Test.Subject.Console3.sln";

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