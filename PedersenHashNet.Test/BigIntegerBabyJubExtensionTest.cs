using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace PedersenHashNet.Test
{
    public class BigIntegerBabyJubExtensionTest
    {
        private const int CaseChunkSize = 100;
        private static readonly BigInteger[][] Cases;

        static BigIntegerBabyJubExtensionTest()
        {
            var lines = File.ReadAllLines("data/test-affine-inverse.tsv");
            Cases = lines
                .Select(line => line.Split().Select(BigInteger.Parse).ToArray())
                .ToArray();
        }

        public static IEnumerable<object[]> GetMainTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(Cases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetMainTestCases))]
        public void MainTestCase(int chunkId)
        {
            var cases = Cases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var num1 = caseData[0];
                var num2 = caseData[1];
                var affineExpected = caseData[2];
                var inverseExpected = caseData[3];

                var affineActual = num1.Affine(num2);
                var inverseActual = num1.Inverse(num2);

                Assert.Equal(affineExpected, affineActual);
                Assert.Equal(inverseExpected, inverseActual);
            }
        }
    }
}