using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

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
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.SdkName, "mobilecenter.uwp");
        }

        /// <summary>
        /// Verify carrier country in device information
        /// </summary>
        [TestMethod]
        public void VerifyCarrierCountry()
        {
            const string CountryCode = "US";
            MobileCenter.SetCountryCode(CountryCode);

            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.CarrierCountry, CountryCode);
        }
    }
}
