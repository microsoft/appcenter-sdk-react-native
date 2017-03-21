using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public bool Enabled { get; set; }

        public void InvokeSuspending()
        {
            if (Enabled)
            {
                ApplicationSuspending?.Invoke(null, null);
            }
        }

        public void InvokeResuming()
        {
            if (Enabled)
            {
                ApplicationResuming?.Invoke(null, null);
            }
        }

        public void InvokeUnhandledException()
        {
            UnhandledExceptionOccurred?.Invoke(null, new UnhandledExceptionOccurredEventArgs(new Exception()));
        }

        public event EventHandler ApplicationSuspending;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
