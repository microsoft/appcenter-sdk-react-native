using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Test.Windows.Storage;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test
{

    [TestClass]
    public class FakeStorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";

        [TestMethod]
        public void ShutdownTimeout()
        {
            var storage = new Mobile.Storage.Storage(storageAdapter);
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.CountLogsAsync(StorageTestChannelName));
            var result = storage.Shutdown(TimeSpan.FromMilliseconds(50));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShutdownSucceed()
        {
            var storage = new Mobile.Storage.Storage(storageAdapter);
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.CountLogsAsync(StorageTestChannelName));
            var result = storage.Shutdown(TimeSpan.FromSeconds(100));

            Assert.IsTrue(result);
        }


        /// <summary>
        /// Verify that if an error occurs while executing a query (from GetLogs), a StorageException gets thrown.
        /// </summary>
        [TestMethod]
        public void GetLogsQueryError()
        {
            var mockAdapter = new Mock<IStorageAdapter>();
            mockAdapter.Setup(adapter => adapter.OpenAsync()).Returns(TaskExtension.GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<object[]>())).Returns(TaskExtension.GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<object[]>())).Throws(new TestDbException(""));

            var fakeStorage = new Mobile.Storage.Storage(mockAdapter.Object);

            var logs = new List<Log>();
            Assert.ThrowsException<StorageException>(() =>
                fakeStorage.GetLogsAsync(StorageTestChannelName, 1, logs).RunNotAsync());
        }

    }
}
