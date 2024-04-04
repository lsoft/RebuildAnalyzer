using Microsoft.Build.Locator;
using RebuildAnalyzer.Helper;
using System.Runtime.CompilerServices;

namespace RebuildAnalyzer.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void ModuleInitialize()
        {
            MSBuildLocator.RegisterDefaults();
            ParallelOption.MaxDegreeOfParallelism = 1;
        }
    }
}