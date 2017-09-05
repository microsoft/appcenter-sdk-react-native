using System;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.Core;

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
            var informationInvalidated = false;

            void InformationInvalidated(object sender, EventArgs e)
            {
                informationInvalidated = true;
            }

            DeviceInformationHelper.InformationInvalidated += InformationInvalidated;
            MobileCenter.SetCountryCode("US");
            DeviceInformationHelper.InformationInvalidated -= InformationInvalidated;
            Assert.AreEqual(informationInvalidated, true);
        }
    }
}
