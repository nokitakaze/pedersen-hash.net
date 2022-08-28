using System;
using BenchmarkDotNet.Running;
using PedersenHashNet.Benchmark.Main;

namespace PedersenHashNet.Benchmark
{
    internal static class Program
    {
        private static void Main()
        {
            var summaryMain = BenchmarkRunner.Run<Generator>();

            Console.WriteLine("==================================");
            Console.WriteLine("==================================");
            Console.WriteLine("==================================");
            Console.WriteLine(summaryMain);
        }
    }
}