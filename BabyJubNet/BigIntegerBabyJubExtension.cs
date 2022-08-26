using System.Numerics;

namespace BabyJubNet
{
    public static class BigIntegerBabyJubExtension
    {
        public static BigInteger Affine(this BigInteger a, BigInteger q)
        {
            var nq = -q;

            var aux = a;
            if (aux < 0)
            {
                if (aux <= nq)
                {
                    aux %= q;
                }

                if (aux < 0)
                {
                    aux += q;
                }

                return aux;
            }

            if (aux >= q)
            {
                aux %= q;
            }

            return aux;
        }

        public static BigInteger Inverse(this BigInteger a, BigInteger q)
        {
            var t = BigInteger.Zero;
            var r = q;
            var newt = BigInteger.One;
            var newr = a.Affine(q);
            while (!newr.IsZero)
            {
                var q1 = r / newr;
                (t, newt) = (newt, t - q1 * newt);
                (r, newr) = (newr, r - q1 * newr);
            }

            if (t < BigInteger.Zero)
            {
                t += q;
            }

            return t;
        }
    }
}