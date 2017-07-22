using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        private static IApplicationLifecycleHelper _instance;
        public static IApplicationLifecycleHelper Instance
        {
            get { return _instance ?? (_instance = new ApplicationLifecycleHelper()); }
            // Setter for testing
            internal set { _instance = value; }
        }

        public bool HasStarted { get; private set; }
        public bool IsSuspended { get; private set; }

        // Internal for testing
        internal void InvokeSuspended()
        {
            IsSuspended = true;
            ApplicationSuspended?.Invoke(null, null);
        }

        // Internal for testing
        internal void InvokeResuming()
        {
            // Need to notify DeviceInformationHelper to refresh display cache here too because there is no guarantee that 
            // it will automatically happen beforehand
            DeviceInformationHelper.RefreshDisplayCache();
            HasStarted = true;
            IsSuspended = false;
            ApplicationResuming?.Invoke(null, null);
        }

        // Internal for testing
        public void InvokeUnhandledException()
        {
            UnhandledExceptionOccurred?.Invoke(null, new UnhandledExceptionOccurredEventArgs(new Exception()));
        }

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
