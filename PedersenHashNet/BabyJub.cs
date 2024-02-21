using System;
using System.Linq;
using System.Numerics;

namespace PedersenHashNet
{
    /// <summary>
    /// BabyJub Curve
    /// </summary>
    /// <remarks>
    /// https://docs.zkbob.com/implementation/elliptic-curve-cryptography
    /// https://eips.ethereum.org/EIPS/eip-2494
    /// </remarks>
    public static class BabyJub
    {
        public static readonly BigInteger p = BN128.r;

        /// <summary>
        /// BabyJub: Coefficient a
        /// </summary>
        public static readonly BigInteger CTA = 168700;

        /// <summary>
        /// BabyJub: Coefficient d
        /// </summary>
        public static readonly BigInteger D = 168696;

        /// <summary>
        /// BabyJub: Order
        /// </summary>
        public static readonly BigInteger order =
            // 0x30644e72e131a029b85045b68181585d59f76dc1c90770533b94bee1c9093788
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

        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        public static (BigInteger A, BigInteger B)? UnpackPoint(byte[] _buff)
        {
            var F = BN128.Fr;

            var buff = _buff.ToArray();
            var sign = false;
            var P = (A: BigInteger.Zero, B: BigInteger.Zero);
            if ((buff[31] & 0x80) > 0)
            {
                sign = true;
                buff[31] = (byte)(buff[31] & 0x7F);
            }

            P.B = new BigInteger(buff, true, false);

            if (P.B >= p)
            {
                return null;
            }

            var y2 = F.Square(P.B);

            var x_prev1 = BigInteger.One - y2;
            var x_prev2 = CTA - ((D * y2) % F.q);
            var x_prev3 = (x_prev1 * x_prev2.Inverse(F.q)) % F.q;
            var x = F.Sqrt(x_prev3);

            if (x == null)
            {
                return null;
            }

            if (sign)
            {
                x = -x;
            }

            P.A = x.Value.Affine(F.q);

            return P;
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

            var value1_a = (CTA * x2) % F.q;
            var value1 = value1_a + y2;

            var value2_a = (x2 * y2) % F.q;
            var value2 = (value2_a * D) % F.q;
            value2 += BigInteger.One;

            var value1_affine = value1.Affine(F.q);
            var value2_affine = value2.Affine(F.q);

            return value1_affine == value2_affine;
        }
    }
}