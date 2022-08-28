using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace PedersenHashNet.Test
{
    public class PedersenHashTest2
    {
        private const int CaseChunkSize = 100;
        private static readonly object[][] TornadoCommitments;

        static PedersenHashTest2()
        {
            var lines = File.ReadAllLines("data/test-tornado-commitments.tsv");
            TornadoCommitments = lines
                .Select(line =>
                {
                    var a = line.Split();
                    var commitment = a[0];
                    var noteKey = a[1];

                    return new object[]
                    {
                        commitment,
                        noteKey
                    };
                })
                .ToArray();
        }

        public static bool NeedShortTest()
        {
            return Environment
                .GetCommandLineArgs()
                .Skip(1)
                .Any(x => x.ToLowerInvariant() == "--short-test");
        }

        #region Pedersen hash

        public static IEnumerable<object[]> GetPedersenTestCases()
        {
            var q = Enumerable
                .Range(0, (int)Math.Ceiling(TornadoCommitments.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId })
                .Take(5); // todo delme

            if (NeedShortTest())
            {
                q = q.Take(5);
            }

            return q.ToArray();
        }

        [Theory]
        [MemberData(nameof(GetPedersenTestCases))]
        public void PedersenTestCase(int chunkId)
        {
            var cases = TornadoCommitments
                .Skip(chunkId * CaseChunkSize)
                .Take(CaseChunkSize)
                .ToArray();
            foreach (var caseData in cases)
            {
                var expectedCommitment = (string)caseData[0];
                var noteKey = (string)caseData[1];

                var actualCommitment1 = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(noteKey);
                Assert.StartsWith("0x", actualCommitment1);
                Assert.Equal(EnsureHexPrefix(expectedCommitment), actualCommitment1);
                actualCommitment1 = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(noteKey[2..]);
                Assert.Equal(EnsureHexPrefix(expectedCommitment), actualCommitment1);

                var noteKeyArray = PedersenHashGenerator.ParseHex(noteKey);
                Assert.Equal(
                    noteKey.ToLowerInvariant(),
                    "0x" + string.Concat(noteKeyArray.Select(t => t.ToString("x2")))
                );
                var actualCommitment2 = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(noteKeyArray);
                Assert.StartsWith("0x", actualCommitment2);
                Assert.Equal(EnsureHexPrefix(expectedCommitment), actualCommitment2);
            }
        }

        private static string EnsureHexPrefix(string value)
        {
            return value.StartsWith("0x") ? value : "0x" + value;
        }

        #endregion

        [Fact]
        private static void CheckMalformedLength_Bytes()
        {
            for (var length = 1; length < 100; length++)
            {
                if (length == 31 * 2)
                {
                    continue;
                }

                var bytes = new byte[length];
                try
                {
                    PedersenHashGenerator.GetCommitmentFromPrivatePair(bytes);
                    Assert.Fail("GetCommitmentFromPrivatePair didn't raise the exception");
                }
                catch (PedersenHashNetException e)
                {
                    Assert.NotEqual(0, e.ErrorCode);
                    Assert.NotEqual(1, e.ErrorCode);
                }
            }
        }

        [Fact]
        private static void CheckMalformedLength_Hex()
        {
            var rnd = new Random();

            for (var length = 1; length < 100; length++)
            {
                if (length == 31 * 2)
                {
                    continue;
                }

                var bytes = new byte[length];
                rnd.NextBytes(bytes);
                var hex = string.Concat(bytes.Select(t => t.ToString("x2")));
                try
                {
                    PedersenHashGenerator.GetCommitmentFromPrivatePair(hex);
                    Assert.Fail("GetCommitmentFromPrivatePair didn't raise the exception");
                }
                catch (PedersenHashNetException e)
                {
                    Assert.NotEqual(0, e.ErrorCode);
                    Assert.NotEqual(1, e.ErrorCode);
                }

                try
                {
                    PedersenHashGenerator.GetCommitmentFromPrivatePair("0x" + hex);
                    Assert.Fail("GetCommitmentFromPrivatePair didn't raise the exception");
                }
                catch (PedersenHashNetException e)
                {
                    Assert.NotEqual(0, e.ErrorCode);
                    Assert.NotEqual(1, e.ErrorCode);
                }
            }
        }
    }
}