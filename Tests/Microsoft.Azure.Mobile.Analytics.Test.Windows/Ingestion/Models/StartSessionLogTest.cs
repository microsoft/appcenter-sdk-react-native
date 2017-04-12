using Moq;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class StartSessionLogTest
    {
        private const long TOffset = 0;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            StartSessionLog emptyLog = new StartSessionLog();
            StartSessionLog log = new StartSessionLog(TOffset, mockDevice.Object);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);
        }
    }
}
