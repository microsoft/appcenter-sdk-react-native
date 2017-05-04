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
            Push.Enabled = false;
            Assert.IsFalse(Push.Enabled);

            Push.Enabled = true;
            Assert.IsTrue(Push.Enabled);
        }
    }
}
