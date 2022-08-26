using System;
using System.Collections.Generic;

namespace BabyJubNet
{
    public static class PedersenHashGenerator
    {
        private const string GENPOINT_PREFIX = "PedersenGenerator";
        private const int windowSize = 4;
        private const int nWindowsPerSegment = 50;

        public static object PedersenHash(byte[] msg)
        {
            const int bitsPerSegment = windowSize * nWindowsPerSegment;

            throw new NotImplementedException();
        }

        private static readonly List<object> bases = new List<object>();

        public static object GetBasePoint(int pointIdx)
        {
            if (pointIdx < bases.Count)
            {
                return bases[pointIdx];
            }

            throw new NotImplementedException();
        }

        public static byte[] Buffer2bits(byte[] buff)
        {
            var res = new byte[buff.Length * 8];
            for (var i = 0; i < buff.Length; i++)
            {
                var b = buff[i];
                res[i * 8] = (byte)(b & 0x01);
                res[i * 8 + 1] = (byte)(b & 0x02);
                res[i * 8 + 2] = (byte)(b & 0x04);
                res[i * 8 + 3] = (byte)(b & 0x08);
                res[i * 8 + 4] = (byte)(b & 0x10);
                res[i * 8 + 5] = (byte)(b & 0x20);
                res[i * 8 + 6] = (byte)(b & 0x40);
                res[i * 8 + 7] = (byte)(b & 0x80);
            }

            return res;
        }
    }
}