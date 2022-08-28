using System;

namespace BabyJubNet
{
    public class BabyJubNetException : Exception
    {
        // ReSharper disable once NotAccessedField.Global
        public readonly int ErrorCode;

        public BabyJubNetException(string errorMsg, int errorCode = 1) : base(errorMsg)
        {
            ErrorCode = errorCode;
        }
    }
}