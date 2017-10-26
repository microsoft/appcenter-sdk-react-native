using Moq;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
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
            var logList = new List<Log> {mockLog.Object, mockLog.Object, mockLog.Object};
            var emptyContainer = new LogContainer();
            var container = new LogContainer(logList);

            Assert.IsNotNull(emptyContainer);
            Assert.IsNotNull(container);
            CollectionAssert.AreEquivalent(logList, (List<Log>)container.Logs);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when Logs == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenLogsIsNull()
        {
            var container = new LogContainer(null);
            Assert.ThrowsException<ValidationException>(() => container.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when Logs.Count is less than 1
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionLogsCountIsLessThan1()
        {
            IList<Log> logList = new List<Log>();
            var container = new LogContainer(logList);
            Assert.ThrowsException<ValidationException>(() => container.Validate());
        }

        /// <summary>
        /// Verify that Validate method call Validate on every item in Logs
        /// </summary>
        [TestMethod]
        public void TestValidateCallValidateOnEveryItem()
        {
            var mockLog = new Mock<Log>();
            IList<Log> logList = new List<Log>
            {
                mockLog.Object,
                mockLog.Object,
                mockLog.Object
            };
            var container = new LogContainer(logList);
            container.Validate();

            mockLog.Verify(log => log.Validate(), Times.Exactly(logList.Count));
        }
    }
}
