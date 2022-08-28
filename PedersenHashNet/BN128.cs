using System.Numerics;

namespace PedersenHashNet
{
    public static class BN128
    {
        // ReSharper disable once NotAccessedField.Global
        public static readonly BigInteger q;

        // ReSharper disable once NotAccessedField.Global
        public static readonly BigInteger r;

        // ReSharper disable once NotAccessedField.Global
        public static readonly ZQField F1;

        // ReSharper disable once NotAccessedField.Global
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