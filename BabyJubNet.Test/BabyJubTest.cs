using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace BabyJubNet.Test
{
    public class BabyJubTest
    {
        private const int CaseChunkSize = 100;
        private static readonly BigInteger[][] EscalarCases;
        private static readonly BigInteger[][] AddPointCases;

        static BabyJubTest()
        {
            var lines = File.ReadAllLines("data/test-mul-escalar.tsv");
            EscalarCases = lines
                .Select(line => line.Split().Select(BigInteger.Parse).ToArray())
                .ToArray();

            lines = File.ReadAllLines("data/test-add-point.tsv");
            AddPointCases = lines
                .Select(line => line.Split().Select(BigInteger.Parse).ToArray())
                .ToArray();
        }

        #region Mul Point Escalar

        public static IEnumerable<object[]> GetEscalarTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(EscalarCases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetEscalarTestCases))]
        public void EscalarTestCase(int chunkId)
        {
            var cases = EscalarCases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var point1a = caseData[0];
                var point1b = caseData[1];
                var e = (int)caseData[2];

                var point2ExpectedA = caseData[3];
                var point2ExpectedB = caseData[4];

                var point2Actual = BabyJub.MulPointEscalar((point1a, point1b), e);

                Assert.Equal(point2ExpectedA, point2Actual.A);
                Assert.Equal(point2ExpectedB, point2Actual.B);
            }
        }

        #endregion

        #region Add Point

        public static IEnumerable<object[]> GetAddPointTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(AddPointCases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetAddPointTestCases))]
        public void AddPointTestCase(int chunkId)
        {
            var cases = AddPointCases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var point1a = caseData[0];
                var point1b = caseData[1];

                var point2a = caseData[2];
                var point2b = caseData[3];

                var point3ExpectedA = caseData[4];
                var point3ExpectedB = caseData[5];

                var point3Actual = BabyJub.AddPoint((point1a, point1b), (point2a, point2b));

                Assert.Equal(point3ExpectedA, point3Actual.A);
                Assert.Equal(point3ExpectedB, point3Actual.B);
            }
        }

        #endregion
    }
}