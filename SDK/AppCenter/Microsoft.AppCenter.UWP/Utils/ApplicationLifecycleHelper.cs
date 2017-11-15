using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Core;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;

        // True if InvokeResuming has been called at least once during the current process
        private static bool _started;
        
        // Considered to be suspended until can verify that has started
        private static bool _suspended = true;

        // Singleton instance of ApplicationLifecycleHelper
        private static ApplicationLifecycleHelper _instance;
        public static ApplicationLifecycleHelper Instance
        {
            get { return _instance ?? (_instance = new ApplicationLifecycleHelper()); }

            // Setter for testing
            internal set { _instance = value; }
        }

        /// <summary>
        /// Indicates whether the application is currently in a suspended state. 
        /// </summary>
        public bool IsSuspended => _suspended;

        public ApplicationLifecycleHelper()
        {
            // Subscribe to Resuming and Suspending events.
            CoreApplication.Suspending += InvokeSuspended;

            // If the "LeavingBackground" event is present, use that for Resuming. Else, use CoreApplication.Resuming.
            if (ApiInformation.IsEventPresent(typeof(CoreApplication).FullName, "LeavingBackground"))
            {
                CoreApplication.LeavingBackground += InvokeResuming;

                // If the application has anything visible, then it has already started,
                // so invoke the resuming event immediately.
                HasStartedAndNeedsResume().ContinueWith(completedTask =>
                {
                    if (completedTask.Result)
                    {
                        InvokeResuming(null, EventArgs.Empty);
                    }
                });
            }
            else
            {
                // In versions of Windows 10 where the LeavingBackground event is unavailable, we condider this point to be
                // the start so invoke resuming (and subscribe to future resume events). If InvokeResuming were not called here,
                // the resuming event wouldn't be invoked until the *next* time the application is resumed, which is a problem
                // if the application is not currently suspended. The side effect is that regardless of whether UI is available
                // ever in the process, InvokeResuming will be called at least once (in the case where LeavingBackground isn't
                // available).
                CoreApplication.Resuming += InvokeResuming;
                InvokeResuming(null, EventArgs.Empty);
            }

            // Subscribe to unhandled errors events.
            CoreApplication.UnhandledErrorDetected += (sender, eventArgs) =>
            {
                try
                {
                    // Intentionally propagate exception to get the exception object that crashed the app.
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

        // Determines whether the application has started already and is not suspended, 
        // but ApplicationLifecycleHelper has not yet fired an initial "resume" event.
        private static async Task<bool> HasStartedAndNeedsResume()
        {
            var needsResume = false;
            try
            {
                // Don't use CurrentSynchronizationContext as that seems to cause an error in Unity applications.
                var asyncAction = CoreApplication.MainView?.CoreWindow?.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        // If started already, a resume has already occurred.
                        if (_started)
                        {
                            return;
                        }
                        if (CoreApplication.Views.Any(view => view.CoreWindow != null &&
                                                              view.CoreWindow.Visible))
                        {
                            needsResume = true;
                        }
                    });
                if (asyncAction != null)
                {
                    await asyncAction;
                }
            }
            catch (Exception e) when (e is COMException || e is InvalidOperationException)
            {
                // If MainView can't be accessed, a COMException or InvalidOperationException is thrown. It means that the
                // MainView hasn't been created, and thus the UI hasn't appeared yet.
                AppCenterLog.Debug(AppCenterLog.LogTag,
                    "Not invoking resume immediately because UI is not ready.");
            }
            return needsResume;
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
    }
}
