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

        public static IEnumerable<object[]> GetAffineInverseTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(Cases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetAffineInverseTestCases))]
        public void AffineInverseTestCase(int chunkId)
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

        [Fact]
        public void AffineZero()
        {
            try
            {
                BigInteger.One.Affine(BigInteger.Zero);
                Assert.Fail("Affine with zero didn't raise an exception");
            }
            catch (System.DivideByZeroException)
            {
                // Goooooood!
            }
        }

        [Fact]
        public void InverseZero()
        {
            try
            {
                BigInteger.One.Inverse(BigInteger.Zero);
                Assert.Fail("Affine with zero didn't raise an exception");
            }
            catch (System.DivideByZeroException)
            {
                // Goooooood!
            }
        }
    }
}