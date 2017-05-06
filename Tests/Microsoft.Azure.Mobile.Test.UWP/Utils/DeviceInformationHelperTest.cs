using System;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.UWP.Utils
{
    [TestClass]
    public class DeviceInformationHelperTest
    {
        /// <summary>
        /// Verify sdk name in device information
        /// </summary>
        [TestMethod]
        public void VerifySdkName()
        {
            var device = new DeviceInformationHelper().GetDeviceInformation();
            Assert.AreEqual(device.SdkName, "mobilecenter.uwp");
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

            var device = new DeviceInformationHelper().GetDeviceInformation();
            Assert.AreEqual(device.CarrierCountry, CountryCode);
        }
    }
}
