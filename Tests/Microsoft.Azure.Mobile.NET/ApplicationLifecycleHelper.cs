using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public bool Enabled { get; set; }

        public void InvokeSuspended()
        {
            if (Enabled)
            {
                ApplicationSuspended?.Invoke(null, null);
            }
        }

        public void InvokeResuming()
        {
            // Need to notify DeviceInformationHelper to refresh display cache here too because there is no guarantee that 
            // it will automatically happen beforehand
            DeviceInformationHelper.RefreshDisplayCache();
            if (Enabled)
            {
                ApplicationResuming?.Invoke(null, null);
            }
        }

        public void InvokeUnhandledException()
        {
            UnhandledExceptionOccurred?.Invoke(null, new UnhandledExceptionOccurredEventArgs(new Exception()));
        }

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
