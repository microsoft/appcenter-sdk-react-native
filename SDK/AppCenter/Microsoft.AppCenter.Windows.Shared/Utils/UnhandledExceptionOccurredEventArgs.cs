using System;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Event argument type for UnhandledException event that is invoked by <see cref="IApplicationLifecycleHelper"/>
    /// </summary>
    public class UnhandledExceptionOccurredEventArgs : EventArgs
    {
        public UnhandledExceptionOccurredEventArgs(Exception e)
        {
            Exception = e;
        }

        /// <summary>
        /// Gets the unhandled exception.
        /// </summary>
        public Exception Exception { get; }
    }
}
