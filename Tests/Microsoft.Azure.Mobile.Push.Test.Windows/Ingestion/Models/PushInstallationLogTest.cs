using System;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Push.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Mobile.Ingestion.Models.Device;

    [TestClass]
    public class PushInstallationLogTest
    {
        private static readonly DateTime? Timestamp = null;

        [TestMethod]
        public void TestConstructor()
        {
            var mockDevice = new Mock<Device>();

            PushInstallationLog log = new PushInstallationLog(Timestamp, mockDevice.Object, "token1");
            Assert.IsNotNull(log);
            Assert.AreEqual(default(System.Guid?), log.Sid);
            Assert.AreEqual("token1", log.PushToken);

            PushInstallationLog log2 = new PushInstallationLog(Timestamp, mockDevice.Object, "token2", System.Guid.NewGuid());
            Assert.IsNotNull(log2);
            Assert.IsNotNull(log2.Sid);
            Assert.AreEqual("token2", log2.PushToken);
        }

        [TestMethod]
        public void TestValidateThrowsExceptionWhenPushTokenIsNull()
        {
            var mockDevice = new Mock<Device>();

            PushInstallationLog log = new PushInstallationLog(Timestamp, mockDevice.Object, null);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }
    }
}
