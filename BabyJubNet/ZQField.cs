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
    }
}