using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace BabyJubNet.Test
{
    public class PedersenHashTest
    {
        private const int CaseChunkSize = 100;
        private static readonly object[][] Cases;

        static PedersenHashTest()
        {
            var lines = File.ReadAllLines("data/test-pedersen-hash.tsv");
            Cases = lines
                .Select(line =>
                {
                    var a = line.Split();
                    var input = ParseHex(a[0]);

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
        }

        private static byte[] ParseHex(string hex)
        {
            if (hex.StartsWith("0x"))
            {
                hex = hex[2..];
            }

            return Enumerable
                .Range(0, hex.Length / 2)
                .Select(i => byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber))
                .ToArray();
        }

        #region Mul Point Escalar

        public static IEnumerable<object[]> GetPedersenTestCases()
        {
            return Enumerable
                .Range(0, (int)Math.Ceiling(Cases.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .ToArray();
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

                var obj = PedersenHashGenerator.PedersenHash(input);

                throw new NotImplementedException();
            }
        }

        #endregion

        [Fact]
        public void GetBasePointTest()
        {
            for (var i = 0; i < 10; i++)
            {
                var point = PedersenHashGenerator.GetBasePoint(i);
                var packed = BabyJub.PackPoint(point);
                var pointRestored = BabyJub.UnpackPoint(packed);
            }
        }
    }
}