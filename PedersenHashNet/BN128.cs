using System.Numerics;

namespace PedersenHashNet
{
    public static class BN128
    {
        public static readonly BigInteger q;

        /// <summary>
        /// The curve finite field modulus
        /// </summary>
        public static readonly BigInteger r;

        public static readonly ZQField F1;

        public static readonly ZQField Fr;

        static BN128()
        {
            // 0x30644e72e131a029b85045b68181585d97816a916871ca8d3c208c16d87cfd47
            q = BigInteger.Parse("21888242871839275222246405745257275088696311157297823662689037894645226208583");
            // 0x30644e72e131a029b85045b68181585d2833e84879b9709143e1f593f0000001
            r = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");
            F1 = new ZQField(q);
            Fr = new ZQField(r);
        }
    }
}