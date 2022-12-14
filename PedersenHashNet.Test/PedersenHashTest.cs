using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace PedersenHashNet.Test
{
    public class PedersenHashTest
    {
        private const int CaseChunkSize = 20;
        private static readonly object[][] Cases;
        private static readonly object[][] PackUnpackCases;

        static PedersenHashTest()
        {
            var lines = File.ReadAllLines("data/test-pedersen-hash.tsv");
            Cases = lines
                .Select(line =>
                {
                    var a = line.Split();
                    var input = PedersenHashGenerator.ParseHex(a[0]);

                    if (a[1].StartsWith("0x"))
                    {
                        a[1] = a[1][2..].ToLowerInvariant();
                    }

                    return new object[]
                    {
                        input,
                        a[1],
                        BigInteger.Parse(a[2]),
                        BigInteger.Parse(a[3])
                    };
                })
                .ToArray();

            lines = File.ReadAllLines("data/test-pack-unpack.tsv");
            PackUnpackCases = lines
                .Select(line =>
                {
                    var a = line.Split();
                    var num = int.Parse(a[0]);

                    return new object[]
                    {
                        num,
                        BigInteger.Parse(a[5]),
                        BigInteger.Parse(a[6])
                    };
                })
                .ToArray();
        }

        #region Pedersen hash

        public static IEnumerable<object[]> GetPedersenTestCases()
        {
            var q = Enumerable
                .Range(0, (int)Math.Ceiling(Cases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId });

            if (Util.IsOpenCoverOrShort())
            {
                q = q.Take(1);
            }
            else if (Util.NeedFullTest())
            {
            }
            else
            {
                q = q.Take(50);
            }

            return q.ToArray();
        }

        [Theory]
        [MemberData(nameof(GetPedersenTestCases))]
        public void PedersenTestCase(int chunkId)
        {
            var cases = Cases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var input = (byte[])caseData[0];
                var expectedPacked = (string)caseData[1];

                var expectedUnpacked1 = (BigInteger)caseData[2];
                var expectedUnpacked2 = (BigInteger)caseData[3];

                var packedHash = PedersenHashGenerator.PedersenHash(input);
                var packedHash_hex = string.Concat(packedHash.Select(t => t.ToString("x2")));
                Assert.Equal(expectedPacked, packedHash_hex);

                var actualUnpacked = BabyJub.UnpackPoint(packedHash);
                Assert.NotNull(actualUnpacked);
                Assert.Equal(expectedUnpacked1, actualUnpacked.Value.A);
                Assert.Equal(expectedUnpacked2, actualUnpacked.Value.B);
            }
        }

        #endregion

        #region Pack Unpack

        public static IEnumerable<object[]> GetGetBasePointTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(PackUnpackCases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetGetBasePointTestCases))]
        public void GetBasePointTestCase(int chunkId)
        {
            var cases = PackUnpackCases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var tryIdx = (int)caseData[0];
                var pointA = (BigInteger)caseData[1];
                var pointB = (BigInteger)caseData[2];

                var pointActual = PedersenHashGenerator.GetBasePoint(tryIdx);
                Assert.Equal(pointA, pointActual.A);
                Assert.Equal(pointB, pointActual.B);
            }
        }

        #endregion

        [Fact]
        public void CheckParseSemiMalformedHex()
        {
            var a1 = PedersenHashGenerator.ParseHex("0x123");
            var b1 = new BigInteger(a1, true, true);
            Assert.Equal(0x0123, (int)b1);

            var a2 = PedersenHashGenerator.ParseHex("0x0123");
            var b2 = new BigInteger(a2, true, true);
            Assert.Equal(0x0123, (int)b2);

            var a3 = PedersenHashGenerator.ParseHex("123");
            var b3 = new BigInteger(a3, true, true);
            Assert.Equal(0x0123, (int)b3);
        }
    }
}