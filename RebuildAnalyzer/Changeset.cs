using System.Runtime.CompilerServices;
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
        private HashSet<string> _changedFiles;

        public bool IsEmpty => _changedFiles.Count == 0;

        /// <summary>
        /// Builds changeset with filepaths in linux format.
        /// If current OS is Windows, convertation / -> \ will be performed.
        /// If current OS is NOT Windows, no convertation occurs.
        /// </summary>
        public static Changeset BuildChangesetFromLinuxPaths(
            IEnumerable<string> changedFiles
            )
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var fixedPaths = changedFiles
                    .Select(r => r.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))
                    ;

                return new Changeset(fixedPaths);
            }

            return new Changeset(changedFiles);
        }

        public Changeset(
            IEnumerable<string> changedFiles
            )
        {
            if (changedFiles is null)
            {
                throw new ArgumentNullException(nameof(changedFiles));
            }

            _changedFiles = new (changedFiles); //uniqueness is essential
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(string filePath)
        {
            return _changedFiles.Contains(filePath);
        }


        public bool ContainsAtAnyLevel(string filePath)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            foreach(var changedFile in _changedFiles)
            {
                if(changedFile.EndsWith(filePath))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsAny(
            IEnumerable<string> projectFiles
            )
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
