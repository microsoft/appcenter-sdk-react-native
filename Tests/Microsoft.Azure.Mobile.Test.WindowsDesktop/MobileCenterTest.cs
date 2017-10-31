using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.WindowsDesktop
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
        /// Verify configure with WindowsDesktop platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            MobileCenter.Configure("windowsdesktop=appsecret");
            Assert.IsTrue(MobileCenter.Configured);
        }
    }
}
