using Microsoft.Build.Evaluation;
using System.Xml;

namespace RebuildAnalyzer.ConsoleApp
{
    /// <summary>
    /// Результат поиска регрессий изменения.
    /// </summary>
    public sealed class ChangingRegressionsContainer
    {
        public const string XmlNodeName_ChangingRegressions = "ChangingRegressions";

        private readonly HashSet<string> _foundRegressions;

        public IReadOnlyCollection<string> FoundRegressions => _foundRegressions;

        public ChangingRegressionsContainer(
            )
        {
            _foundRegressions = new();
        }

        public static object? Build(
            Project evaluatedProject,
            object? foundChangingRegressions
            )
        {
            var container = (foundChangingRegressions as ChangingRegressionsContainer) ?? new ChangingRegressionsContainer();

            container.Apply(evaluatedProject);

            return container;
        }

        private void Apply(Project evaluatedProject)
        {
            var foundRegressionsList = evaluatedProject
                .AllEvaluatedProperties
                .Where(p => p.Name == XmlNodeName_ChangingRegressions)
                .ToList()
                ;

            foreach (var foundRegressions in foundRegressionsList)
            {
                var xml = new XmlDocument();
                xml.LoadXml($"<?xml version=\"1.0\" encoding=\"utf-8\"?><{XmlNodeName_ChangingRegressions}>{foundRegressions.Xml.Value}</{XmlNodeName_ChangingRegressions}>");

                var foundRegressionNodeList = xml.SelectNodes("/ChangingRegressions/ChangingRegression");
                if (foundRegressionNodeList is not null)
                {
                    foreach (XmlNode foundRegressionNode in foundRegressionNodeList)
                    {
                        var foundRegressionText = foundRegressionNode.InnerText;
                        _foundRegressions.Add(foundRegressionText);
                    }
                }
            }
        }
    }

}
