using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.Core;

namespace Microsoft.AppCenter.Test.UWP
{
    [TestClass]
    public class AppCenterTest
    {
        private AggregateException _unobservedTaskException;

        [TestInitialize]
        public void InitializeAppCenterTest()
        {
            _unobservedTaskException = null;
            AppCenter.Instance = null;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        [TestCleanup]
        public void CleanupAppCenterTest()
        {
            // The UnobservedTaskException will only happen if a Task gets collected by the GC with an exception unobserved
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

            if (_unobservedTaskException != null)
            {
                throw _unobservedTaskException;
            }
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _unobservedTaskException = e.Exception;
        }

        /// <summary>
        /// Verify configure with UWP platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AppCenter.Configure("uwp=appsecret");
            }).AsTask().GetAwaiter().GetResult();

            Assert.IsTrue(AppCenter.Configured);
        }

        /// <summary>
        /// Verify country code setter
        /// </summary>
        [TestMethod]
        public void SetCountryCode()
        {
            var informationInvalidated = false;

            void InformationInvalidated(object sender, EventArgs e)
            {
                informationInvalidated = true;
            }

            DeviceInformationHelper.InformationInvalidated += InformationInvalidated;
            AppCenter.SetCountryCode("US");
            DeviceInformationHelper.InformationInvalidated -= InformationInvalidated;
            Assert.AreEqual(informationInvalidated, true);
        }

        /// <summary>
        /// Verify multiple unhandled exceptions handling
        /// </summary>
        [TestMethod]
        public void MultipleUnhandledExceptions()
        {
            AppCenter.Start("uwp=appsecret");
            Assert.IsTrue(AppCenter.Configured);

            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());

            // _channelGroup.ShutdownAsync() called as "fire and forget", so no way to wait for it correctly.
            Task.Delay(100).Wait();
            
            // Call again.
            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());
            Task.Delay(100).Wait();

            // No exception throws.
        }

        /// <summary>
        /// Start service after unhandled exception
        /// </summary>
        [TestMethod]
        public void StartServiceAfterUnhandledException()
        {
            AppCenter.Configure("uwp=appsecret");
            Assert.IsTrue(AppCenter.Configured);

            // Some exception occurred.
            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());

            // Start any service.
            AppCenter.Start(typeof(TestAppCenterService));

            // No exception throws, just log message.
        }
    }
}
