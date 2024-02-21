using System.Numerics;

namespace PedersenHashNet
{
    public static class BigIntegerBabyJubExtension
    {
        /// <summary>
        /// Applies the affine transformation to a BigInteger value within the specified modulus.
        /// </summary>
        /// <param name="a">The value to transform</param>
        /// <param name="q">The modulus</param>
        /// <returns>The transformed value within the modulus</returns>
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

        /// <summary>
        /// Calculates the multiplicative inverse of a BigInteger modulo q
        /// </summary>
        /// <param name="a">The BigInteger value</param>
        /// <param name="q">The modulus value</param>
        /// <returns>The multiplicative inverse of <paramref name="a"/> modulo <paramref name="q"/>.</returns>
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