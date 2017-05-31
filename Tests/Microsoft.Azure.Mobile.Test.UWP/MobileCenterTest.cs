using System;
using Windows.ApplicationModel.Activation;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.Core;
using Moq;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Test.MockServices;

namespace Microsoft.Azure.Mobile.Test.UWP
{
    [TestClass]
    public class MobileCenterTest
    {
        [TestInitialize]
        public void InitializeMobileCenterTest()
        {
            MobileCenter.Instance = null;
        }

        /// <summary>
        /// Verify configure with UWP platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MobileCenter.Configure("uwp=appsecret");
            }).AsTask().GetAwaiter().GetResult();

            Assert.IsTrue(MobileCenter.Configured);
        }

        /// <summary>
        /// Verify country code setter
        /// </summary>
        [TestMethod]
        public void SetCountryCode()
        {
            const string CountryCode = "US";
            int informationInvalidated = 0;
            EventHandler OnInformationInvalidated = delegate { informationInvalidated++; };
            DeviceInformationHelper.InformationInvalidated += OnInformationInvalidated;
            MobileCenter.SetCountryCode(CountryCode);
            MobileCenter.SetCountryCode("INVALID");
            DeviceInformationHelper.InformationInvalidated -= OnInformationInvalidated;
            Assert.AreEqual(informationInvalidated, 1);
        }

        /// <summary>
        /// Verify that all services are notified, even if one throws an exception
        /// </summary>
        [TestMethod]
        public void NotifyOnLaunched()
        {
            var mockApplicationLifecycleHelper = new Mock<ApplicationLifecycleHelper>();
            ApplicationLifecycleHelper.Instance = mockApplicationLifecycleHelper.Object;

            // This test assumes that the order in which NotifyOnLaunched is invoked on services equals the
            // order in which they were passed to the start method
            MobileCenter.Start("appsecret", typeof(GoodMockService), typeof(ThrowsOnNotifyOnLaunchedMockService));
            MobileCenter.NotifyOnLaunched(default(LaunchActivatedEventArgs));

            mockApplicationLifecycleHelper.Verify(lifecycleHelper => lifecycleHelper.NotifyOnLaunched(), Times.Once());
            GoodMockService.Instance.MockServiceInstance.Verify(service => service.NotifyOnLaunched(It.IsAny<LaunchActivatedEventArgs>()), Times.Once());
        }
    }
}
