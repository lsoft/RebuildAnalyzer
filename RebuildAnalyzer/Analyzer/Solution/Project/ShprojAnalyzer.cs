using System.Xml.Serialization;
using RebuildAnalyzer.FileStructure;
using RebuildAnalyzer.Helper;

namespace RebuildAnalyzer.Analyzer.Solution.Project
{
    public sealed class ShprojAnalyzer : IProjectAnalyzer
    {
        public string RootFolder { get; }
        public string ShprojRelativeFilePath { get; }

        public string ShprojFullFilePath => Path.Combine(RootFolder, ShprojRelativeFilePath);

        public string ProjectRelativeFilePath => ShprojRelativeFilePath;

        public ShprojAnalyzer(
            string rootFolder,
            string shprojRelativeFilePath
            )
        {
            if (shprojRelativeFilePath is null)
            {
                throw new ArgumentNullException(nameof(shprojRelativeFilePath));
            }

            RootFolder = rootFolder;
            ShprojRelativeFilePath = shprojRelativeFilePath;
        }

        public void Prepare()
        {
            //nothing to do
        }

        public bool IsAffected(Changeset changeset)
        {
            if (changeset.Contains(ShprojRelativeFilePath))
            {
                //сам shproj изменился
                return true;
            }

            //shared items included into target project by itself
            //we do not need to analyze them by special way
            return false;
        }
    }
}
