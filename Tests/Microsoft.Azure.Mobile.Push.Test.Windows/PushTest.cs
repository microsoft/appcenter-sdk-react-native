using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Push.Test.Windows
{
    [TestClass]
    public class PushTest
    {
        [TestInitialize]
        public void InitializeAnalyticsTest()
        {
            Push.Instance = new Push();
        }

        [TestMethod]
        public void InstanceIsNotNull()
        {
            Push.Instance = null;
            Assert.IsNotNull(Push.Instance);
        }

        [TestMethod]
        public void GetEnabled()
        {
            Push.PlatformEnabled = false;
            Assert.IsFalse(Push.PlatformEnabled);

            Push.PlatformEnabled = true;
            Assert.IsTrue(Push.PlatformEnabled);
        }
    }
}
