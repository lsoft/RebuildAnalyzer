using RebuildAnalyzer.Analyzer.Solution;
using RebuildAnalyzer.Analyzer.Solution.Project;

namespace RebuildAnalyzer.Analyzer
{
    public abstract class RepositoryAnalyzer
    {
        public const string DirectoryBuildprops = "Directory.Build.props";
        public const string DirectoryPackagesprops = "Directory.Packages.props";

        protected abstract IReadOnlyList<string> GetSkippedProjects();

        protected abstract IReadOnlyList<AnalyzeSubject> ScanForSubjects();

        public IReadOnlyList<AnalyzeSubject> DetermineAffectedSubjects(Changeset changeset)
        {
            if(changeset.IsEmpty)
            {
                return new List<AnalyzeSubject>();
            }

            var subjects = ScanForSubjects();

            if (changeset.ContainsAtAnyLevel(DirectoryBuildprops))
            {
                //все субъекты затронуты
                return subjects;
            }
            if (changeset.ContainsAtAnyLevel(DirectoryPackagesprops))
            {
                //все субъекты затронуты
                return subjects;
            }
            //TODO: Добавить еще файлы, которые влияют на множество sln, slnf, csproj

            var affectedSubjects = new List<AnalyzeSubject>();

            var skippedProjects = GetSkippedProjects();
            foreach (var subject in subjects)
            {
                switch (subject.Kind)
                {
                    case AnalyzeSubjectKindEnum.Sln:
                        var slna = new SlnAnalyzer(skippedProjects, subject.RootFolder, subject.RelativeFilePath);
                        if (slna.IsAffected(changeset))
                        {
                            affectedSubjects.Add(subject);
                        }
                        break;
                    case AnalyzeSubjectKindEnum.Slnf:
                        var slnfa = new SlnfAnalyzer(skippedProjects, subject.RootFolder, subject.RelativeFilePath);
                        if (slnfa.IsAffected(changeset))
                        {
                            affectedSubjects.Add(subject);
                        }
                        break;
                    case AnalyzeSubjectKindEnum.Project:
                        if (!skippedProjects.Contains(subject.RelativeFilePath))
                        {
                            var pa = ProjectAnalyzerFactory.TryCreate(
                                subject.RootFolder,
                                subject.RelativeFilePath
                                );
                            if (pa != null && pa.IsAffected(changeset))
                            {
                                affectedSubjects.Add(subject);
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException(subject.Kind.ToString());
                }
            }

            return affectedSubjects;
        }
    }
}
