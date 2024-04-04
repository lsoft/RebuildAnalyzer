﻿namespace RebuildAnalyzer
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
        public IReadOnlySet<string> ChangedFiles { get; }

        public bool IsEmpty => ChangedFiles.Count == 0;

        public Changeset(
            IReadOnlySet<string> changedFiles
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

        public bool ContainsAny(IReadOnlySet<string> projectFiles)
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