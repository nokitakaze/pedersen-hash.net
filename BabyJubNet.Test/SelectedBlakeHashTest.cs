using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace BabyJubNet.Test
{
    public class SelectedBlakeHashTest
    {
        private const int CaseChunkSize = 100;
        private static readonly object[][] PackUnpackCases;

        static SelectedBlakeHashTest()
        {
            var lines = File.ReadAllLines("data/test-pack-unpack.tsv");
            PackUnpackCases = lines
                .Select(line =>
                {
                    var a = line.Split();

                    return new object[]
                    {
                        a[1],
                        PedersenHashGenerator.ParseHex(a[2])
                    };
                })
                .ToArray();
        }

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
            var cases = PackUnpackCases
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var S = (string)caseData[0];
                var h = (byte[])caseData[1];
                var h_hex = string.Concat(h.Select(t => t.ToString("x2")));

                var sByte = Encoding.ASCII.GetBytes(S);

                using var hashAlgorithm = new Konscious.Security.Cryptography.HMACBlake2B(256);
                hashAlgorithm.Initialize();
                var h1 = hashAlgorithm.ComputeHash(sByte);
                h1![31] &= 0xBF;

                var h1_hex = string.Concat(h1.Select(t => t.ToString("x2")));
                Assert.Equal(h_hex, h1_hex);
            }
        }

        #endregion
    }
}