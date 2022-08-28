using System;
using System.Linq;

namespace PedersenHashNet.Test
{
    internal static class Util
    {
        public static bool IsOpenCoverOrShort()
        {
            if (Environment
                .GetCommandLineArgs()
                .Skip(1)
                .Any(x => x.ToLowerInvariant() == "--short-test"))
            {
                return true;
            }

            if (Environment.GetCommandLineArgs()[0].EndsWith("testhost.dll") ||
                Environment.GetCommandLineArgs()[0].EndsWith("testhost.exe"))
            {
                var corProfiler = Environment.GetEnvironmentVariable("Cor_Profiler_Path_64");
                return corProfiler?.ToLowerInvariant().EndsWith("opencover.profiler.dll") ?? false;
            }

            return false;
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