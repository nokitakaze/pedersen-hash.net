using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace PedersenHashNet.Test
{
    public class BabyJubTest
    {
        private const int CaseChunkSize = 100;
        private static readonly BigInteger[][] EscalarCases;
        private static readonly BigInteger[][] AddPointCases;
        private static readonly object[][] PackUnpackCases;

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

            lines = File.ReadAllLines("data/test-pack-unpack.tsv");
            PackUnpackCases = lines
                .Select(line =>
                {
                    var a = line.Split();
                    var num = int.Parse(a[0]);

                    return new object[]
                    {
                        num,
                        ParseHex(a[2]),
                        BigInteger.Parse(a[3]),
                        BigInteger.Parse(a[4])
                    };
                })
                .ToArray();
        }

        private static byte[] ParseHex(string hex)
        {
            if (hex.StartsWith("0x"))
            {
                hex = hex[2..];
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            return Enumerable
                .Range(0, hex.Length / 2)
                .Select(i => byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber))
                .ToArray();
        }

        #region Mul Point Escalar

        public static IEnumerable<object[]> GetEscalarTestCases()
        {
            var q = Enumerable
                .Range(0, (int)Math.Ceiling(EscalarCases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId });
            if (Util.IsOpenCoverOrShort())
            {
                q = q.Take(5);
            }

            return q.ToArray();
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
            var q = Enumerable
                .Range(0, (int)Math.Ceiling(AddPointCases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId });

            if (Util.IsOpenCoverOrShort())
            {
                q = q.Take(5);
            }

            return q.ToArray();
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

        #region Pack Unpack

        public static IEnumerable<object[]> GetPackUnpackTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(PackUnpackCases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetPackUnpackTestCases))]
        public void PackUnpackTestCase(int chunkId)
        {
            const string GENPOINT_PREFIX = "PedersenGenerator";

            var cases = PackUnpackCases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var pointIdx = (int)caseData[0];
                var h = (byte[])caseData[1];
                var pA = (BigInteger)caseData[2];
                var pB = (BigInteger)caseData[3];

                var h_hex = string.Concat(h.Select(t => t.ToString("x2")));

                for (var tryIdx = 0;; tryIdx++)
                {
                    var S = string.Format("{0}_{1}_{2}",
                        GENPOINT_PREFIX,
                        pointIdx.ToString().PadLeft(32, '0'),
                        tryIdx.ToString().PadLeft(32, '0')
                    );
                    var sByte = Encoding.ASCII.GetBytes(S);

                    using var hashAlgorithm = new BlakeSharpNG.Blake256();
                    hashAlgorithm.Initialize();
                    hashAlgorithm.ComputeHash(sByte);
                    var h1 = hashAlgorithm.Hash;
                    h1![31] &= 0xBF;

                    var temp = BabyJub.UnpackPoint(h1);
                    if (temp != null)
                    {
                        var h1_hex = string.Concat(h1.Select(t => t.ToString("x2")));
                        Assert.Equal(h_hex, h1_hex);

                        break;
                    }
                }

                var actualUnpacked = BabyJub.UnpackPoint(h);
                Assert.NotNull(actualUnpacked);

                Assert.Equal(pA, actualUnpacked.Value.A);
                Assert.Equal(pB, actualUnpacked.Value.B);

                var actualPacked = BabyJub.PackPoint((pA, pB));
                var actualPacked_hex = string.Concat(actualPacked.Select(t => t.ToString("x2")));
                Assert.Equal(h_hex, actualPacked_hex);

                // hint: This may seem like a redundant test.
                // But this is a test in case the problem is in the data of the test cases themselves.
                var repacked = BabyJub.PackPoint((actualUnpacked.Value.A, actualUnpacked.Value.B));
                var repacked_hex = string.Concat(repacked.Select(t => t.ToString("x2")));
                Assert.Equal(h_hex, repacked_hex);
            }
        }

        #endregion

        [Fact]
        public void NotInBabyJubCurve()
        {
            var rnd = new Random();
            var h = new byte[32];
            var validPoints = new List<(BigInteger A, BigInteger B)>();
            while (validPoints.Count < 1000)
            {
                rnd.NextBytes(h);
                var p = BabyJub.UnpackPoint(h);
                if (p != null)
                {
                    validPoints.Add(p.Value);
                }
            }

            foreach (var point in validPoints)
            {
                var p = (point.A + 1, point.B);
                Assert.False(BabyJub.InSubgroup(p));

                var bigBytesA = new byte[point.A.ToByteArray().Length];
                var bigBytesB = new byte[point.B.ToByteArray().Length];

                for (var i = 0; i < 100; i++)
                {
                    rnd.NextBytes(bigBytesA);
                    p = (new BigInteger(bigBytesA, true, false), point.B);
                    Assert.False(BabyJub.InSubgroup(p));

                    rnd.NextBytes(bigBytesB);
                    p = (point.A, new BigInteger(bigBytesB, true, false));
                    Assert.False(BabyJub.InSubgroup(p));
                }
            }
        }
    }
}