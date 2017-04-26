using Moq;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class PushInstallationLogTest
    {
        private const long TOffset = 0;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            var mockDevice = new Mock<Device>();

            PushInstallationLog log = new PushInstallationLog(TOffset, mockDevice.Object, "token1");

            Assert.IsNotNull(log);
            Assert.AreEqual(default(System.Guid?), log.Sid);
            Assert.AreEqual("token1", log.PushToken);

            PushInstallationLog log2 = new PushInstallationLog(TOffset, mockDevice.Object, "token2", System.Guid.NewGuid());
            Assert.IsNotNull(log2);
            Assert.IsNotNull(log2.Sid);
            Assert.AreEqual("token2", log2.PushToken);
        }
    }
}
