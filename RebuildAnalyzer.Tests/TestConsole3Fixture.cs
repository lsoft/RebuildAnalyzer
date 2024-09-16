using RebuildAnalyzer.Analyzer;
using RebuildAnalyzer.ConsoleApp;

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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console3_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(2, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Common23_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(3, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
            Assert.AreEqual("Regression text Common23 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Third());
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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console3_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(2, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
        }

        private sealed class RepositoryAnalyzer : Analyzer.RepositoryAnalyzer
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