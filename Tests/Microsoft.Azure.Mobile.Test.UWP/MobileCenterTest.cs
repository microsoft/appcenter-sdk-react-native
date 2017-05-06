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
    }
}
