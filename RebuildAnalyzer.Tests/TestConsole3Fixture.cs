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

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console3_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
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

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Common23_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
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

            var tra = new RepositoryAnalyzer();
            var ar = tra.DetermineAffectedSubjects(changeset);

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console3_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
        }

        public sealed class RepositoryAnalyzer : Analyzer.RepositoryAnalyzer
        {
            public const string SlnRelativeFilePath = "RebuildAnalyzer.Test.Subject.Console3.sln";
            public const string Project_Console3_RelativeFilePath = @"RebuildAnalyzer.Test.Subject.Console3\RebuildAnalyzer.Test.Subject.Console3.csproj";
            public const string Project_Common23_RelativeFilePath = @"RebuildAnalyzer.Test.Subject.Common23\RebuildAnalyzer.Test.Subject.Common23.csproj";

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