using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        private static bool _started;
        private static bool _suspended;
        private bool _needsSubscribeToCoreWindowActivatedEvent = true;

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
            const string errorMessage = "Failed to fully initialize ApplicationLifecycleHelper at this point. This is not necessarily an error.";
            CoreApplication.Resuming += InvokeResuming;
            CoreApplication.Suspending += InvokeSuspended;
            try
            {
                CoreApplication.MainView.CoreWindow.Activated += InvokeStarted;
                if (CoreApplication.Views.Count > 0)
                {
                    _started = true;
                }
            }
            catch (COMException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, errorMessage);
            }
            catch (ArgumentException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, errorMessage);
            }
            Application.Current.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs(eventArgs.Exception));
            };
        }

        /// <summary>
        /// Indicates that OnLaunched has occurred, which is useful when initialization begins at an earlier step in the lifecycle
        /// </summary>
        /// <remarks>Virtual for testing</remarks>
        public virtual void NotifyOnLaunched()
        {
            if (_needsSubscribeToCoreWindowActivatedEvent)
            {
                // Don't log a message because it might be confusing
                return;
            }
            try
            {
                CoreApplication.MainView.CoreWindow.Activated += InvokeStarted;
                if (CoreApplication.Views.Count > 0)
                {
                    _started = true;
                }
                _needsSubscribeToCoreWindowActivatedEvent = false;
            }
            catch (COMException)
            {
                throw new MobileCenterException("Failed to initialize ApplicationLifecycleHelper; are you accessing Mobile Center from your App() constructor? Initialization should be done in OnLaunched()/OnStart().");
            }
            catch (ArgumentException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag,
                    "Failed to complete initialization of ApplicationLifecycleHelper. Please ensure that you are calling MobileCenter.NotifyOnLaunched() from the OnLaunched() method.");
            }
        }

        private void InvokeResuming(object sender, object e)
        {
            _suspended = false;
            ApplicationResuming?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeStarted(object sender, object e)
        {
            // Should only have a single invocation of the started event, so unsubscribe here
            CoreApplication.MainView.CoreWindow.Activated -= InvokeStarted;
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
