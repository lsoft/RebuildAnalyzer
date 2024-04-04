using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebuildAnalyzer.Helper
{
    public static class PathHelper
    {
        public static string GetFullPath(
            string rootPath,
            string somePath
            )
        {
            var rooted = Path.IsPathRooted(somePath);

            var result = rooted
                ? somePath
                : Path.Combine(rootPath, somePath);

            //на случай путей типа ../../somefile.cs
            result = Path.GetFullPath(result);

            return result;
        }
    }
}
