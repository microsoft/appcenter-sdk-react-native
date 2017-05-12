using System;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            MobileCenter.Configure("uwp=appsecret");
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
