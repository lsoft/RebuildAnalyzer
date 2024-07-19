namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public interface IProjectAnalyzer
    {
        string ProjectRelativeFilePath { get; }

        
        void Prepare();

        Changeset? IsAffected(Changeset changeset);
    }
}
