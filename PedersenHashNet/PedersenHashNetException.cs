using System;

namespace PedersenHashNet
{
    public class PedersenHashNetException : Exception
    {
        // ReSharper disable once NotAccessedField.Global
        public readonly int ErrorCode;

        public PedersenHashNetException(string errorMsg, int errorCode = -1) : base(errorMsg)
        {
            ErrorCode = errorCode;
        }
    }
}