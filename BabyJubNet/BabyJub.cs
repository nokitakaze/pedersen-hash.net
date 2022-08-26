using System;
using System.Linq;
using System.Numerics;

namespace BabyJubNet
{
    public static class BabyJub
    {
        public static readonly BigInteger p = BN128.r;
        public static readonly BigInteger CTA = 168700;
        public static readonly BigInteger D = 168696;

        public static readonly BigInteger order =
            BigInteger.Parse("21888242871839275222246405745257275088614511777268538073601725287587578984328");

        public static readonly BigInteger subOrder = order >> 3;

        public static (BigInteger A, BigInteger B) AddPoint(
            (BigInteger a, BigInteger b) a,
            (BigInteger a, BigInteger b) b
        )
        {
            var resA = ((a.a * b.b + b.a * a.b) * (BigInteger.One + D * a.a * b.a * a.b * b.b).Inverse(p))
                .Affine(p);

            var resB = ((a.b * b.b - CTA * a.a * b.a) * (BigInteger.One - D * a.a * b.a * a.b * b.b).Inverse(p))
                .Affine(p);

            return (resA, resB);
        }

        public static (BigInteger A, BigInteger B) MulPointEscalar(
            (BigInteger a, BigInteger b) basePoint,
            BigInteger e
        )
        {
            var res = (BigInteger.Zero, BigInteger.One);
            var rem = e;
            var exp = basePoint;

            while (!rem.IsZero)
            {
                if (!rem.IsEven)
                {
                    res = AddPoint(res, exp);
                }

                exp = AddPoint(exp, exp);
                rem >>= 1;
            }

            return res;
        }

        public static byte[] PackPoint((BigInteger A, BigInteger B) P)
        {
            var buff = P.B.ToByteArray(true, false);
            if (buff.Length < 32)
            {
                var bb = new byte[32];
                Array.Copy(buff, 0, bb, 0, buff.Length);
                buff = bb;
            }

            if (P.A > (p >> 1))
            {
                buff[31] = (byte)(buff[31] | 0x80);
            }

            return buff;
        }

        public static (BigInteger A, BigInteger B) UnpackPoint(byte[] _buff)
        {
            throw new NotImplementedException();
        }

        public static bool InSubgroup((BigInteger A, BigInteger B) P)
        {
            if (!InCurve(P))
            {
                return false;
            }

            var res = MulPointEscalar(P, subOrder);
            return (res.A == BigInteger.Zero) && (res.B == BigInteger.One);
        }

        public static bool InCurve((BigInteger A, BigInteger B) P)
        {
            var F = BN128.Fr;

            var x2 = F.Square(P.A);
            var y2 = F.Square(P.B);

            var value1 = (CTA * x2) + y2;
            var value2 = BigInteger.One + ((x2 * y2) * D);

            return value1 == value2;
        }
    }
}