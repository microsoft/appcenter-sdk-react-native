using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;

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
        public bool HasStarted => _started;
        
        /// <summary>
        /// Indicates whether the application is currently in a suspended state. This
        /// value can only really be known once "HasStarted" is true
        /// </summary>
        public bool IsSuspended => _suspended;

        public ApplicationLifecycleHelper()
        {
            // Subscribe to Resuming and Suspending events
            CoreApplication.Suspending += InvokeSuspended;

            if (ApiInformation.IsEventPresent(typeof(CoreApplication).FullName, "LeavingBackground"))
            {
                CoreApplication.LeavingBackground += InvokeResuming;
                // If the application has anything visible, then it has already started,
                // so invoke the resuming event immediately
                foreach (var view in CoreApplication.Views)
                {
                    if (view.CoreWindow.Visible)
                    {
                        InvokeResuming(null, EventArgs.Empty);
                        break;
                    }
                }
                _suspended = !_started;
            }
            else
            {
                // In versions of Windows 10 where the LeavingBackground event is unavailable, we condider this point to be
                // the start so invoke resuming (and subscribe to future resume events)
                CoreApplication.Resuming += InvokeResuming;
                TriggerResumingIfUiAvailable();
            }

            // Subscribe to unhandled errors events
            CoreApplication.UnhandledErrorDetected += (sender, eventArgs) =>
            {
                try
                {
                    // Intentionally propagating exception to get the exception object that crashed the app.
                    eventArgs.UnhandledError.Propagate();
                }
                catch (Exception exception)
                {
                    UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs(exception));

                    // Since UnhandledError.Propagate marks the error as Handled, rethrow in order to only Log and not Handle.
                    throw;
                }
            };
        }

        private void TriggerResumingIfUiAvailable()
        {
            try
            {
               var _ = CoreApplication.MainView?.CoreWindow?.Dispatcher.RunAsync(
                   CoreDispatcherPriority.Normal, () =>
                    {
                        // If started while this task was queued, return.
                        if (_started)
                        {
                            return;
                        }
                        foreach (var view in CoreApplication.Views)
                        {
                            if (view.CoreWindow != null && view.CoreWindow.Visible)
                            {
                                // Don't need to trigger the events on UI thread
                                Task.Run(() => InvokeResuming(null, EventArgs.Empty));
                                return;
                            }
                        }
                    });
            }
            catch (COMException)
            {
                // If MainView can't be accessed, it hasn't been created, and thus
                // the UI hasn't appeared yet.
            }
        }

        private void InvokeResuming(object sender, object e)
        {
            _started = true;
            _suspended = false;
            ApplicationResuming?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeSuspended(object sender, object e)
        {
            _suspended = true;
            ApplicationSuspended?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
