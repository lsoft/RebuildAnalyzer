namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public interface IProjectAnalyzer
    {
        string RelativeProjectFilePath { get; }

        bool IsAffected(Changeset changeset);
    }
}
