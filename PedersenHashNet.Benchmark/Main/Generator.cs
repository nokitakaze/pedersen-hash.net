using System;
using BenchmarkDotNet.Attributes;

namespace PedersenHashNet.Benchmark.Main
{
    public class Generator
    {
        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        [Params(1, 10, 100, 1000)]
        public int N = 0;

        private byte[] bytesForHashing;

        [GlobalSetup]
        public void Setup()
        {
            var rnd = new Random();
            bytesForHashing = new byte[PedersenHashGenerator.SecretKeyLength];
            rnd.NextBytes(bytesForHashing);
        }

        [Benchmark]
        public void GetCommitmentFromPrivatePair()
        {
            for (var i = 0; i < N; i++)
            {
                PedersenHashGenerator.GetCommitmentFromPrivatePair(bytesForHashing);
            }
        }

        [Benchmark]
        public void GenerateCommitmentPair()
        {
            for (var i = 0; i < N; i++)
            {
                PedersenHashGenerator.GenerateCommitmentPair();
            }
        }
    }
}