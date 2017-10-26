using System;

namespace Microsoft.AppCenter
{
    public class AppCenterException : Exception
    {
        public AppCenterException(string message) : base(message) { }
        public AppCenterException(string message, Exception innerException) : base(message, innerException) { }
    }
}
