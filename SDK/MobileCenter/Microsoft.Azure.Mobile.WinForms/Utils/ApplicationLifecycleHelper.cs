using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public ApplicationLifecycleHelper()
        {
            Enabled = true;
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs((Exception)eventArgs.ExceptionObject));
            };
        }

        public bool Enabled { get; set; }

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
