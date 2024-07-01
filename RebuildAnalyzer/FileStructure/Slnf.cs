
using System.Runtime.InteropServices;

namespace RebuildAnalyzer.FileStructure
{
    public class SlnfJson
    {
        public SlnfSolutionJson solution { get; set; }

        internal void UpdateSlashesInPaths()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                solution.ChangeSlashesToLinuxStyle();
            }
        }
    }

    public class SlnfSolutionJson
    {
        /// <summary>
        /// Path to sln file.
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// Relative paths to included csprojes.
        /// Paths here are relative against sln file NOT a slnF file!
        /// </summary>
        public string[] projects { get; set; }

        internal void ChangeSlashesToLinuxStyle()
        {
            path = path.Replace(
                "\\",
                "/"
                );

            for (var pi = 0; pi < projects.Length; pi++)
            {
                projects[pi] = projects[pi].Replace(
                    "\\",
                    "/"
                    );
            }
        }
    }
}
