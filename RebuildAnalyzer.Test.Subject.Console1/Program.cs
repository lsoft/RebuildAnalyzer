namespace RebuildAnalyzer.Test.Subject.Console1
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if RELEASE
            ReleaseFile.Log();
#else
            Console.WriteLine("DEBUG");
#endif

#if NET7_0
            DotNet7File.Log();
#else
            Console.WriteLine("DotNet 8");
#endif
        }
    }
}
