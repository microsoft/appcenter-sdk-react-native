using Moq;
using Microsoft.Rest;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class LogWithPopertiesTest
    {
        private const long TOffset = 0;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            LogWithProperties emptyLog = new TestLogWithProperties();
            LogWithProperties log = new TestLogWithProperties(TOffset, mockDevice.Object);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);
            Assert.IsNull(log.Properties);
        }
    }


    class TestLogWithProperties : LogWithProperties
    {
        public TestLogWithProperties() { }
        public TestLogWithProperties(long toffset, Device device, System.Guid? sid = default(System.Guid?), IDictionary<string, string> properties = default(IDictionary<string, string>))
            : base(toffset, device, sid, properties) { }
    }
}
