using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class UnhandledExceptionOccurredEventArgs : EventArgs
    {
        public UnhandledExceptionOccurredEventArgs(Exception e)
        {
            Exception = e;
        }
        public Exception Exception { get; }
    }
}
