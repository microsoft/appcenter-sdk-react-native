using Moq;
using Microsoft.Rest;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    [TestClass]
    public class LogContainerTest
    {
        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockLog = new Mock<Log>();

            IList<Log> logList = new List<Log>();
            logList.Add(mockLog.Object);
            logList.Add(mockLog.Object);
            logList.Add(mockLog.Object);

            LogContainer emptyLog = new LogContainer();
            LogContainer log = new LogContainer(logList);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);

            Assert.AreEqual(logList, log.Logs);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when Logs == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenLogsIsNull()
        {
            IList<Log> logList = null;
            LogContainer container = new LogContainer(logList);
            Assert.ThrowsException<ValidationException>(() => container.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when Logs.Count is less than 1
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionLogsCountIsLessThan1()
        {
            IList<Log> logList = new List<Log>();
            LogContainer container = new LogContainer(logList);
            Assert.ThrowsException<ValidationException>(() => container.Validate());
        }

        /// <summary>
        /// Verify that Validate method call Validate on every item in Logs
        /// </summary>
        [TestMethod]
        public void TestValidateCallValidateOnEveryItem()
        {
            var mockLog = new Mock<Log>();
            IList<Log> logList = new List<Log>();
            logList.Add(mockLog.Object);
            logList.Add(mockLog.Object);
            logList.Add(mockLog.Object);

            LogContainer container = new LogContainer(logList);
            container.Validate();

            mockLog.Verify(log => log.Validate(), Times.Exactly(logList.Count));
        }
    }
}
