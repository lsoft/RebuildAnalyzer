using System.Xml.Serialization;
using RebuildAnalyzer.Analyzer.Request;
using RebuildAnalyzer.Analyzer.Result;
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

        public void Prepare(
            AnalyzeRequest request
            )
        {
            //nothing to do
        }

        public AffectedSubjectPart? IsAffected(
            AnalyzeRequest request
            )
        {
            if (request.Changeset.Contains(ShprojRelativeFilePath))
            {
                //сам shproj изменился
                return new AffectedSubjectPart(
                    ProjectRelativeFilePath,
                    new Changeset(ShprojRelativeFilePath),
                    null
                    );
            }

            //shared items included into target project by itself
            //we do not need to analyze them by special way
            return null;
        }
    }
}
