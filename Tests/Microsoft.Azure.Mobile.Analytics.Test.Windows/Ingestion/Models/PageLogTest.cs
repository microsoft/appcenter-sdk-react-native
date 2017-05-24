using Moq;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class PageLogTest
    {
        private const long TOffset = 0;
        private const string Name = "Name";

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            PageLog emptyLog = new PageLog();
            PageLog log = new PageLog(TOffset, mockDevice.Object, Name);

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

            PageLog log = new PageLog(TOffset, mockDevice.Object, NullName);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }
    }
}
