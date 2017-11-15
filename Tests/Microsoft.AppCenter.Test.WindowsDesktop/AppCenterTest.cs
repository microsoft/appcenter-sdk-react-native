using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.WindowsDesktop
{
    [TestClass]
    public class AppCenterTest
    {
        [TestInitialize]
        public void InitializeAppCenterTest()
        {
            AppCenter.Instance = null;
        }

        /// <summary>
        /// Verify configure with WindowsDesktop platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            AppCenter.Configure("windowsdesktop=appsecret");
            Assert.IsTrue(AppCenter.Configured);
        }
    }
}
