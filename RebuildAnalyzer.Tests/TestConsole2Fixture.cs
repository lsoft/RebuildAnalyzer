using RebuildAnalyzer.Analyzer;
using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.ConsoleApp;

namespace RebuildAnalyzer.Tests
{
    public class TestConsole2Fixture
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
                    "RebuildAnalyzer.Test.Subject.Console2\\Program.cs"
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
            Assert.AreEqual(RepositoryAnalyzer.Project_Console2_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(3, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
            Assert.AreEqual("Regression text Console2 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Third());
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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console2_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(3, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
            Assert.AreEqual("Regression text Console2 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Third());
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

        private sealed class RepositoryAnalyzer : Analyzer.RepositoryAnalyzer
        {
            public const string SlnRelativeFilePath = "RebuildAnalyzer.Test.Subject.Console2.sln";
            public const string Project_Console2_RelativeFilePath = @"RebuildAnalyzer.Test.Subject.Console2\RebuildAnalyzer.Test.Subject.Console2.csproj";
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