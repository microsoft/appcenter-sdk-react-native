using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test
{
    using PredType = Expression<Func<Mobile.Storage.Storage.LogEntry, bool>>;

    [TestClass]
    public class FakeStorageTest
    {
        private const string StorageTestChannelName = "storageTestChannelName";

        /// <summary>
        /// Verify that shutdown fails when tasks exceed time limit
        /// </summary>
        [TestMethod]
        public void ShutdownTimeout()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            mockConnection.Setup(
                    c => c.InsertAsync(It.IsAny<Mobile.Storage.Storage.LogEntry>()))
                .Callback(() => Task.Delay(TimeSpan.MaxValue).Wait())
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            var result = storage.Shutdown(TimeSpan.FromTicks(1));

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verify that shutdown is successful when tasks complete in time
        /// </summary>
        [TestMethod]
        public void ShutdownSucceed()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            mockConnection.Setup(
                    c => c.InsertAsync(It.IsAny<Mobile.Storage.Storage.LogEntry>()))
                .Callback(() => Task.Delay(TimeSpan.FromSeconds(2)))
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            var result = storage.Shutdown(TimeSpan.FromSeconds(100));
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verify that new tasks are not started after shutdown
        /// </summary>
        [TestMethod]
        public void ShutdownPreventsNewTasks()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            Assert.IsTrue(storage.Shutdown(TimeSpan.FromSeconds(10)));
            Assert.ThrowsException<StorageException>(
                () => storage.GetLogsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<Log>>()).RunNotAsync());
        }

        /// <summary>
        /// Verify that if an error occurs while executing a query (from GetLogs), a StorageException gets thrown.
        /// </summary>
        [TestMethod]
        public void GetLogsQueryError()
        {
            var mockAdapter = new Mock<IStorageAdapter>();
            mockAdapter.Setup(
                    a => a.GetAsync(It.IsAny<PredType>(), It.IsAny<int>()))
                .Throws(new StorageException());
            var fakeStorage = new Mobile.Storage.Storage(mockAdapter.Object);
            var logs = new List<Log>();
            Assert.ThrowsException<StorageException>(() =>
                fakeStorage.GetLogsAsync(StorageTestChannelName, 1, logs).RunNotAsync());
        }

        /// <summary>
        /// Verify that storage throws StorageException if something went wrong
        /// </summary>
        [TestMethod]
        public void StorageThrowsStorageException()
        {
            var mockAdapter = new Mock<IStorageAdapter>();
            mockAdapter.Setup(
                    a => a.GetAsync(It.IsAny<PredType>(), It.IsAny<int>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.InsertAsync(It.IsAny<Mobile.Storage.Storage.LogEntry>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.DeleteAsync(It.IsAny<Expression<Func<Mobile.Storage.Storage.LogEntry, bool>>>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.CountAsync(It.IsAny<Expression<Func<Mobile.Storage.Storage.LogEntry, bool>>>()))
                .Throws(new StorageException());
            var fakeStorage = new Mobile.Storage.Storage(mockAdapter.Object);

            Assert.ThrowsException<StorageException>(() => fakeStorage.PutLogAsync("channel_name", new TestLog()).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.DeleteLogsAsync("channel_name", string.Empty).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.CountLogsAsync("channel_name").RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.GetLogsAsync("channel_name", 1, new List<Log>()).RunNotAsync());
        }
    }
}
