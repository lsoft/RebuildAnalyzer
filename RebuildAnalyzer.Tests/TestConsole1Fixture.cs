using RebuildAnalyzer.Analyzer;
using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.ConsoleApp;

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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(2, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(2, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
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
            var ar = tra.DetermineAffectedSubjects(
                new AnalyzeRequest(
                    changeset,
                    ChangingRegressionsContainer.Build
                    )
                );

            Assert.AreEqual(1, ar.Results.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ar.Results[0].Subject.RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.Count);
            Assert.AreEqual(changeset.ChangedFiles.First(), ar.Results[0].AffectedParts[0].Changeset.ChangedFiles.First());

            Assert.IsNotNull(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.IsInstanceOf<ChangingRegressionsContainer>(ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults);
            Assert.AreEqual(2, ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Count);
            Assert.AreEqual("Regression text Common 0", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.First());
            Assert.AreEqual("Regression text Common 1", ((ChangingRegressionsContainer)ar.Results[0].AffectedParts[0].AdditionalAnalyzerResults).FoundRegressions.Second());
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

            var ass = ar.Results.Select(a => a.Subject).ToList();

            Assert.AreEqual(1, ass.Count);
            Assert.AreEqual(RepositoryAnalyzer.SlnRelativeFilePath, ass[0].RelativeFilePath);
            Assert.AreEqual(1, ar.Results[0].AffectedParts.Count);
            Assert.AreEqual(RepositoryAnalyzer.Project_Console1_RelativeFilePath, ar.Results[0].AffectedParts[0].RelativeProjectFilePath);
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