using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Push.Test.Windows
{
    [TestClass]
    public class PushTest
    {
        [TestInitialize]
        public void InitializePushTest()
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
            Push.SetEnabled(false);
            Assert.IsFalse(Push.IsEnabledAsync().Result);

            Push.SetEnabled(true);
            Assert.IsTrue(Push.IsEnabledAsync().Result);
        }
    }
}
