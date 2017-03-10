using System;

namespace Microsoft.Azure.Mobile
{
    public class MobileCenterException : Exception
    {
        public MobileCenterException(string message) : base(message) { }
        public MobileCenterException(string message, Exception innerException) : base(message, innerException) { }
    }
}
