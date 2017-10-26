using System;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    using Device = Mobile.Ingestion.Models.Device;

    [TestClass]
    public class PageLogTest
    {
        private static readonly DateTime? Timestamp = null;
        private const string Name = "Name";

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            PageLog emptyLog = new PageLog();
            PageLog log = new PageLog(Timestamp, mockDevice.Object, Name);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);

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

            PageLog log = new PageLog(Timestamp, mockDevice.Object, NullName);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }
    }
}
