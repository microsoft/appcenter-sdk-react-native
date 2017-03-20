using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public delegate void UnhandledExceptionOccurredEventHandler(object sender, UnhandledExceptionOccurredEventArgs e);

    public class UnhandledExceptionOccurredEventArgs : EventArgs
    {
        public UnhandledExceptionOccurredEventArgs(Exception e)
        {
            Exception = e;
        }
        public Exception Exception { get; }
    }
}
