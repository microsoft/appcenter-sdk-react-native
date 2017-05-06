using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.UWP.Utils
{
    [TestClass]
    public class DeviceInformationHelperTest
    {
        [TestMethod]
        public void VerifySdkName()
        {
            var device = new DeviceInformationHelper().GetDeviceInformation();
            Assert.AreEqual(device.SdkName, "mobilecenter.uwp");
        }
    }
}
