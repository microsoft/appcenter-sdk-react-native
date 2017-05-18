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
            const string CountryCode = "US";
            int informationInvalidated = 0;
            EventHandler OnInformationInvalidated = delegate { informationInvalidated++; };
            DeviceInformationHelper.InformationInvalidated += OnInformationInvalidated;
            MobileCenter.SetCountryCode(CountryCode);
            MobileCenter.SetCountryCode("INVALID");
            DeviceInformationHelper.InformationInvalidated -= OnInformationInvalidated;
            Assert.AreEqual(informationInvalidated, 1);
        }
    }
}
