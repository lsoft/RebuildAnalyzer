using System.Xml.Serialization;

namespace RebuildAnalyzer.Helper
{
    public static class XmlSerializerHelper
    {
        public static T? DeserializeFromFile<T>(this XmlSerializer xmlSerializer, string filePath)
        {
            using var reader = new StringReader(File.ReadAllText(filePath));
            var r = (T?)xmlSerializer.Deserialize(reader);
            return r;
        }
    }
}
