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

        #region Pedersen hash

        public static IEnumerable<object[]> GetPedersenTestCases()
        {
            var q = Enumerable
                .Range(0, (int)Math.Ceiling(TornadoCommitments.Length * (1d / CaseChunkSize)))
                .Select(chunkId => new object[] { chunkId });

            if (Util.NeedShortTest())
            {
                q = q.Take(1);
            }
            else if (Util.NeedFullTest())
            {
            }
            else
            {
                q = q.Take(10);
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
                var expectedCommitment = EnsureHexPrefix((string)caseData[0]).ToLowerInvariant();
                var noteKey = EnsureHexPrefix((string)caseData[1]).ToLowerInvariant();

                //
                var actualCommitment1 = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(noteKey);
                Assert.StartsWith("0x", actualCommitment1);
                Assert.Equal(expectedCommitment, actualCommitment1);
                actualCommitment1 = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(noteKey[2..]);
                Assert.Equal(expectedCommitment, actualCommitment1);

                var noteKeyArray = PedersenHashGenerator.ParseHex(noteKey);
                Assert.Equal(
                    noteKey,
                    "0x" + string.Concat(noteKeyArray.Select(t => t.ToString("x2")))
                );
                var actualCommitment2 = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(noteKeyArray);
                Assert.StartsWith("0x", actualCommitment2);
                Assert.Equal(expectedCommitment, actualCommitment2);
            }
        }

        private static string EnsureHexPrefix(string value)
        {
            return value.StartsWith("0x") ? value : "0x" + value;
        }

        #endregion

        #region Malformed input

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

        #endregion

        #region

        [Fact]
        public void TestGenerateCommitmentPair()
        {
            var maxCount = Util.NeedShortTest() ? 10 : 100;
            for (var i = 0; i < maxCount; i++)
            {
                var (secretKey, publicKey) = PedersenHashGenerator.GenerateCommitmentPair();
                Assert.StartsWith("0x", secretKey);
                Assert.Equal(2 + 2 * PedersenHashGenerator.SecretKeyLength, secretKey.Length);
                Assert.StartsWith("0x", publicKey);
                Assert.Equal(2 + 2 * PedersenHashGenerator.PublicKeyLength, publicKey.Length);

                var recalculatedPublic = PedersenHashGenerator.GetHexCommitmentFromPrivatePair(secretKey);
                Assert.Equal(recalculatedPublic, publicKey);
            }
        }

        #endregion
    }
}