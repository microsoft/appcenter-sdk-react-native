using Moq;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class LogTest
    {
        private const long TOffset = 0;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();
            
            Log emptyLog = new TestLog();
            Log log = new TestLog(TOffset, mockDevice.Object);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);

            Assert.AreEqual(default(System.Guid?), log.Sid);
            Assert.AreEqual(TOffset, log.Toffset);
            Assert.AreEqual(mockDevice.Object, log.Device);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when device == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenDeviceIsNull()
        {
            const Device nullDevice = null;
            Log log = new TestLog(TOffset, nullDevice);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }

        /// <summary>
        /// Verify that Validate calls Device.Validate when device != null.
        /// </summary>
        [TestMethod]
        public void TestValidateCallsDeviceValidateWhenDeviceIsNotNull()
        {
            var mockDevice = new Mock<Device>();
            Log log = new TestLog(TOffset, mockDevice.Object);
            log.Validate();

            mockDevice.Verify(device => device.Validate());
        }
    }
}
