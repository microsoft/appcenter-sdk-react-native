using System;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [TestClass]
    public class LogTest
    {
        private static readonly DateTime? Timestamp = null;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();
            
            Log emptyLog = new TestLog();
            Log log = new TestLog(Timestamp, mockDevice.Object);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);

            Assert.AreEqual(default(Guid?), log.Sid);
            Assert.AreEqual(Timestamp, log.Timestamp);
            Assert.AreEqual(mockDevice.Object, log.Device);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when device == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenDeviceIsNull()
        {
            const Device nullDevice = null;
            Log log = new TestLog(Timestamp, nullDevice);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }

        /// <summary>
        /// Verify that Validate calls Device.Validate when device != null.
        /// </summary>
        [TestMethod]
        public void TestValidateCallsDeviceValidateWhenDeviceIsNotNull()
        {
            var mockDevice = new Mock<Device>();
            Log log = new TestLog(Timestamp, mockDevice.Object);
            log.Validate();

            mockDevice.Verify(device => device.Validate());
        }
    }
}
