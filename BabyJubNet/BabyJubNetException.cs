using System;

namespace BabyJubNet
{
    public class BabyJubNetException : Exception
    {
        public BabyJubNetException(string errorMsg) : base(errorMsg)
        {
        }
    }
}