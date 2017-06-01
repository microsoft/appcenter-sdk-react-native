using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        private static bool _started;
        private static bool _suspended;

        // Singleton instance of ApplicationLifecycleHelper
        private static ApplicationLifecycleHelper _instance;
        public static ApplicationLifecycleHelper Instance
        {
            get { return _instance ?? (_instance = new ApplicationLifecycleHelper()); }

            // Setter for testing
            internal set { _instance = value; }
        }

        /// <summary>
        /// Indicates whether the application has shown UI
        /// </summary>
        public bool HasShownWindow => _started;
        
        /// <summary>
        /// Indicates whether the application is currently in a suspended state
        /// </summary>
        public bool IsSuspended => _suspended;

        public ApplicationLifecycleHelper()
        {
            // Subscribe to Resuming and Suspending events
            CoreApplication.Resuming += InvokeResuming;
            CoreApplication.Suspending += InvokeSuspended;

            if (ApiInformation.IsEventPresent(typeof(CoreApplication).FullName, "LeavingBackground"))
            {
                CoreApplication.LeavingBackground += InvokeStarted;
            }
            else
            {
                // In versions of Windows 10 where the LeavingBackground event is unavailable, we condider this point to be
                // the start
                _started = true;
            }
            Application.Current.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs(eventArgs.Exception));
            };
        }

        private void InvokeResuming(object sender, object e)
        {
            _suspended = false;
            ApplicationResuming?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeStarted(object sender, object e)
        {
            CoreApplication.LeavingBackground -= InvokeStarted;
            _started = true;
            _suspended = false;
            ApplicationStarted?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeSuspended(object sender, object e)
        {
            _suspended = true;
            ApplicationSuspended?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler ApplicationStarted;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
