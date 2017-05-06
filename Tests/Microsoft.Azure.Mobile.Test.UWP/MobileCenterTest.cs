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

        [TestMethod]
        public void VerifyPlatformId()
        {
            MobileCenter.Configure("uwp=appsecret");
            Assert.IsTrue(MobileCenter.Configured);
        }
    }
}
