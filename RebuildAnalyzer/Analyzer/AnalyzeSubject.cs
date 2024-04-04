using System.Diagnostics;

namespace RebuildAnalyzer.Analyzer
{
    [DebuggerDisplay("{RelativeFilePath}")]
    public sealed class AnalyzeSubject
    {
        public string RootFolder { get; }
        public string RelativeFilePath { get; }
        public AnalyzeSubjectKindEnum Kind { get; }

        public string FullFilePath => Path.Combine(RootFolder, RelativeFilePath);

        public AnalyzeSubject(
            string rootFolder,
            string relativeFilePath,
            AnalyzeSubjectKindEnum kind
            )
        {
            RootFolder = rootFolder;
            RelativeFilePath = relativeFilePath;
            Kind = kind;
        }

    }

    public enum AnalyzeSubjectKindEnum
    {
        Sln = 0,
        Slnf = 1,
        Project = 2
    }

}
