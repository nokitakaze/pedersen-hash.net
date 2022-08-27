using System.Numerics;

namespace BabyJubNet
{
    public class ZQField
    {
        public readonly BigInteger q;

        public readonly BigInteger minusone;

        public readonly BigInteger nqr;
        public readonly BigInteger s;
        public readonly BigInteger t;
        public readonly BigInteger nqr_to_t;

        public ZQField(BigInteger q)
        {
            this.q = q;
            minusone = q - 1;

            var e = minusone >> 1;
            nqr = 2;

            var r = Exp(this.nqr, e);
            while (r != this.minusone)
            {
                this.nqr += BigInteger.One;
                r = Exp(this.nqr, e);
            }

            this.s = BigInteger.Zero;
            this.t = this.minusone;

            while (this.t.IsEven)
            {
                this.s += BigInteger.One;
                this.t >>= 1;
            }

            this.nqr_to_t = Exp(this.nqr, this.t);
        }

        public BigInteger Exp(BigInteger baseNumber, BigInteger e)
        {
            var res = BigInteger.One;
            var rem = e;
            var exp = baseNumber;

            while (!rem.IsZero)
            {
                if (!rem.IsEven)
                {
                    res = (res * exp) % q;
                }

                exp = (exp * exp) % q;
                rem >>= 1;
            }

            return res;
        }

        public BigInteger Square(BigInteger value)
        {
            return (value * value) % q;
        }

        public BigInteger? Sqrt(BigInteger n)
        {
            n = n.Affine(q);

            if (n.IsZero)
            {
                return BigInteger.Zero;
            }

            // Test that have solution
            var res = this.Exp(n, this.minusone >> 1);
            if (res != BigInteger.One)
            {
                return null;
            }

            var m = (int)this.s;
            var c = this.nqr_to_t;
            var t1 = this.Exp(n, this.t);
            var r = this.Exp(n, (this.t + BigInteger.One) >> 1);

            while (t1 != BigInteger.One)
            {
                var sq = this.Square(t1);
                var i = 1;
                while (sq != BigInteger.One)
                {
                    i++;
                    sq = this.Square(sq);
                }

                // b = c ^ m-i-1
                var b = c;
                for (var j = 0; j < m - i - 1; j++)
                {
                    b = this.Square(b);
                }

                m = i;
                c = this.Square(b);
                t1 = (t1 * c) % q;
                r = (r * b) % q;
            }

            if (r > (this.q >> 1))
            {
                r = -r;
            }

            return r;
        }
    }
}