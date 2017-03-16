using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    [TestClass]
    public class LogFactoryTest
    {
        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestLogFactoryCreate()
        {
            LogFactory<TestLog> logFactory = new LogFactory<TestLog>();

            Log log = logFactory.Create();

            Assert.IsNotNull(log);
            Assert.AreEqual(typeof(TestLog), logFactory.LogType);
        }
    }
}
