using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace BabyJubNet.Test
{
    public class ZQFieldTest
    {
        private const int CaseChunkSize = 100;
        private static readonly BigInteger[][] Cases;

        static ZQFieldTest()
        {
            var lines = File.ReadAllLines("data/test-f1.tsv");
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
                var q = caseData[0];
                var nqr = caseData[2];
                var t = caseData[3];
                var nqr_to_t = caseData[4];

                var field = new ZQField(q);
                Assert.Equal(nqr, field.nqr);
                Assert.Equal(t, field.t);
                Assert.Equal(nqr_to_t, field.nqr_to_t);
            }
        }

        [Fact]
        public void SqrtZero()
        {
            var sqrt = BN128.F1.Sqrt(BigInteger.Zero);
            Assert.NotNull(sqrt);
            Assert.True(sqrt.Value.IsZero);
        }
    }
}