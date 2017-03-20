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

        public event EventHandler<object> ApplicationSuspending;
        public event EventHandler<object> ApplicationResuming;
        public event UnhandledExceptionOccurredEventHandler UnhandledExceptionOccurred;
    }
}
