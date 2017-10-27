using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [TestClass]
    public class LogWithPopertiesTest
    {
        private static readonly DateTime? Timestamp = null;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            LogWithProperties emptyLog = new TestLogWithProperties();
            LogWithProperties log = new TestLogWithProperties(Timestamp, mockDevice.Object);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);
            Assert.IsNull(log.Properties);
        }
    }


    class TestLogWithProperties : LogWithProperties
    {
        public TestLogWithProperties() { }
        public TestLogWithProperties(DateTime? timestamp, Device device, Guid? sid = default(Guid?), IDictionary<string, string> properties = default(IDictionary<string, string>))
            : base(timestamp, device, sid, properties) { }
    }
}
