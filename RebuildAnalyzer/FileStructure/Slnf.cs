namespace RebuildAnalyzer.FileStructure
{
    public class SlnfJson
    {
        public SlnfSolutionJson solution { get; set; }
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
    }
}
