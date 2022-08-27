using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BabyJubNet
{
    public static class PedersenHashGenerator
    {
        private const string GENPOINT_PREFIX = "PedersenGenerator";
        private const int windowSize = 4;
        private const int nWindowsPerSegment = 50;

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public static byte[] PedersenHash(byte[] msg)
        {
            const int bitsPerSegment = windowSize * nWindowsPerSegment;
            var bits = Buffer2bits(msg);

            var nSegments = (int)Math.Floor((bits.Length - 1) * 1d / (windowSize * nWindowsPerSegment)) + 1;

            var accP = (BigInteger.Zero, BigInteger.One);

            for (var s = 0; s < nSegments; s++)
            {
                int nWindows;
                if (s == nSegments - 1)
                {
                    nWindows =
                        (int)Math.Floor(((bits.Length - (nSegments - 1) * bitsPerSegment * 1d) - 1) / windowSize) + 1;
                }
                else
                {
                    nWindows = nWindowsPerSegment;
                }

                var escalar = BigInteger.Zero;
                var exp = BigInteger.One;
                for (var w = 0; w < nWindows; w++)
                {
                    var o = s * bitsPerSegment + w * windowSize;
                    var acc = BigInteger.One;
                    for (var b = 0; ((b < windowSize - 1) && (o < bits.Length)); b++)
                    {
                        if (bits[o])
                        {
                            acc += BigInteger.One << b;
                        }

                        o++;
                    }

                    if (o < bits.Length)
                    {
                        if (bits[o])
                        {
                            acc = -acc;
                        }

                        // ReSharper disable once RedundantAssignment
                        o++;
                    }

                    escalar += acc * exp;
                    exp <<= (windowSize + 1);
                }

                if (escalar < BigInteger.Zero)
                {
                    escalar += BabyJub.subOrder;
                }

                accP = BabyJub.AddPoint(accP, BabyJub.MulPointEscalar(GetBasePoint(s), escalar));
            }

            return BabyJub.PackPoint(accP);
        }

        private static readonly Dictionary<int, (BigInteger A, BigInteger B)> bases =
            new Dictionary<int, (BigInteger A, BigInteger B)>();

        public static (BigInteger A, BigInteger B) GetBasePoint(int pointIdx)
        {
            if (bases.ContainsKey(pointIdx))
            {
                return bases[pointIdx];
            }

            (BigInteger a, BigInteger b)? p = null;
            var tryIdx = 0;
            while (p == null)
            {
                var S = string.Format("{0}_{1}_{2}",
                    GENPOINT_PREFIX,
                    pointIdx.ToString().PadLeft(32, '0'),
                    tryIdx.ToString().PadLeft(32, '0')
                );
                var sByte = Encoding.ASCII.GetBytes(S);

                using var hashAlgorithm = new BlakeSharp.Blake256();
                hashAlgorithm.Initialize();
                hashAlgorithm.ComputeHash(sByte);
                var h = hashAlgorithm.Hash;

                h[31] &= 0xBF; // Set 255th bit to 0 (256th is the signal and 254th is the last possible bit to 1)
                p = BabyJub.UnpackPoint(h);
                tryIdx++;
            }

            var p8 = BabyJub.MulPointEscalar(p.Value, 8);
            Console.WriteLine("{0}\t\t{1}", p8.A, p8.B);

            if (!BabyJub.InSubgroup(p8))
            {
                throw new BabyJubNetException("Point not in curve");
            }

            bases[pointIdx] = p8;
            return p8;
        }

        public static bool[] Buffer2bits(byte[] buff)
        {
            var res = new bool[buff.Length * 8];
            for (var i = 0; i < buff.Length; i++)
            {
                var b = buff[i];
                res[i * 8] = (b & 0x01) > 0;
                res[i * 8 + 1] = (b & 0x02) > 0;
                res[i * 8 + 2] = (b & 0x04) > 0;
                res[i * 8 + 3] = (b & 0x08) > 0;
                res[i * 8 + 4] = (b & 0x10) > 0;
                res[i * 8 + 5] = (b & 0x20) > 0;
                res[i * 8 + 6] = (b & 0x40) > 0;
                res[i * 8 + 7] = (b & 0x80) > 0;
            }

            return res;
        }

        public static byte[] ParseHex(string hex)
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
    }
}