using System;
using System.Linq;

namespace PedersenHashNet.Test
{
    internal static class Util
    {
        public static bool NeedShortTest()
        {
            return Environment.GetCommandLineArgs()[0].Contains("OpenCover.Console.exe") || Environment
                .GetCommandLineArgs()
                .Skip(1)
                .Any(x => x.ToLowerInvariant() == "--short-test");
        }

        public static bool NeedFullTest()
        {
            return Environment
                .GetCommandLineArgs()
                .Skip(1)
                .Any(x => x.ToLowerInvariant() == "--full-test");
        }
    }
}