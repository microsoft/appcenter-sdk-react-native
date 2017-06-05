using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.WindowsClassic
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
        /// Verify configure with WindowsClassic platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            MobileCenter.Configure("windowsclassicdesktop=appsecret");
            Assert.IsTrue(MobileCenter.Configured);
        }
    }
}
