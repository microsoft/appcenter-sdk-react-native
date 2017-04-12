using Moq;
using System;
using Microsoft.Rest;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class EventLogTest
    {
        private const long      TOffset = 0;
        private const string    Name    = "Name";
        private readonly Guid   Id      = Guid.Empty;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            EventLog emptyLog = new EventLog();
            EventLog log = new EventLog(TOffset, mockDevice.Object, Id, Name);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);

            Assert.AreEqual(Id, log.Id);
            Assert.AreEqual(Name, log.Name);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when Name == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenNameIsNull()
        {
            const string NullName = null;
            var mockDevice = new Mock<Device>();

            EventLog log = new EventLog(TOffset, mockDevice.Object, Id, NullName);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }
    }
}
