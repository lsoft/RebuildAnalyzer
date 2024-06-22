using System.Runtime.InteropServices;

namespace RebuildAnalyzer
{
    /// <summary>
    /// Change set of files (from a commit(s) or PR).
    /// </summary>
    public sealed class Changeset
    {
        /// <summary>
        /// File paths to changed (added, modified, deleted) files.
        /// All paths are relative from repository root without leading (back)slash.
        /// For example: src\MyProject\MyProject.csproj or src/MyProject/MyProject.csproj
        /// </summary>
        public ICollection<string> ChangedFiles { get; }

        public bool IsEmpty => ChangedFiles.Count == 0;

        /// <summary>
        /// Builds changeset with filepaths in linux format.
        /// If current OS is Windows, convertation / -> \ will be performed.
        /// If current OS is NOT Windows, no convertation occurs.
        /// </summary>
        public static Changeset BuildChangesetFromLinuxPaths(
            ICollection<string> changedFiles
            )
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var fixedPaths = changedFiles
                    .Select(r => r.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))
                    .ToList();

                return new Changeset(fixedPaths);
            }

            return new Changeset(changedFiles);
        }

        public Changeset(
            ICollection<string> changedFiles
            )
        {
            if (changedFiles is null)
            {
                throw new ArgumentNullException(nameof(changedFiles));
            }

            ChangedFiles = changedFiles;
        }

        public bool Contains(string filePath)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return ChangedFiles.Contains(filePath);
        }


        public bool ContainsAtAnyLevel(string filePath)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            foreach(var changedFile in ChangedFiles)
            {
                if(changedFile.EndsWith(filePath))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsAny(ICollection<string> projectFiles)
        {
            foreach(var projectFile in projectFiles)
            {
                if(Contains(projectFile))
                {
                    return true;
                }
            }

            return false;
        }
    }



}
