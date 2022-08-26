using System.Numerics;

namespace BabyJubNet
{
    public static class BN128
    {
        public static readonly BigInteger q;
        public static readonly BigInteger r;
        public static readonly ZQField F1;
        public static readonly ZQField Fr;

        static BN128()
        {
            q = BigInteger.Parse("21888242871839275222246405745257275088696311157297823662689037894645226208583");
            r = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");
            F1 = new ZQField(q);
            Fr = new ZQField(r);
        }
    }
}