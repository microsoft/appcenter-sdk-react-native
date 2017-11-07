using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test
{
    using PredType = Expression<Func<Microsoft.AppCenter.Storage.Storage.LogEntry, bool>>;

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
                    c => c.InsertAsync(It.IsAny<Microsoft.AppCenter.Storage.Storage.LogEntry>()))
                .Callback(() => Task.Delay(TimeSpan.FromDays(1)).Wait())
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Microsoft.AppCenter.Storage.Storage(mockConnection.Object);

            // Ignore warnings because we just want to "fire and forget"
#pragma warning disable 4014
            storage.PutLog(StorageTestChannelName, new TestLog());
            storage.PutLog(StorageTestChannelName, new TestLog());
#pragma warning restore 4014

            var result = storage.ShutdownAsync(TimeSpan.FromTicks(1)).RunNotAsync();
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
                    c => c.InsertAsync(It.IsAny<Microsoft.AppCenter.Storage.Storage.LogEntry>()))
                .Callback(() => Task.Delay(TimeSpan.FromSeconds(2)).Wait())
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Microsoft.AppCenter.Storage.Storage(mockConnection.Object);
            
            // Ignore warnings because we just want to "fire and forget"
#pragma warning disable 4014
            storage.PutLog(StorageTestChannelName, new TestLog());
            storage.PutLog(StorageTestChannelName, new TestLog());
#pragma warning restore 4014

            var result = storage.ShutdownAsync(TimeSpan.FromSeconds(100)).RunNotAsync();
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verify that new tasks are not started after shutdown
        /// </summary>
        [TestMethod]
        public void ShutdownPreventsNewTasks()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            var storage = new Microsoft.AppCenter.Storage.Storage(mockConnection.Object);
            var result = storage.ShutdownAsync(TimeSpan.FromSeconds(10)).RunNotAsync();
            Assert.IsTrue(result);
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
            var fakeStorage = new Microsoft.AppCenter.Storage.Storage(mockAdapter.Object);
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
            mockAdapter.Setup(c => c.InsertAsync(It.IsAny<Microsoft.AppCenter.Storage.Storage.LogEntry>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.DeleteAsync(It.IsAny<Expression<Func<Microsoft.AppCenter.Storage.Storage.LogEntry, bool>>>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.CountAsync(It.IsAny<Expression<Func<Microsoft.AppCenter.Storage.Storage.LogEntry, bool>>>()))
                .Throws(new StorageException());
            var fakeStorage = new Microsoft.AppCenter.Storage.Storage(mockAdapter.Object);

            Assert.ThrowsException<StorageException>(() => fakeStorage.PutLog("channel_name", new TestLog()).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.DeleteLogs("channel_name", string.Empty).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.CountLogsAsync("channel_name").RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.GetLogsAsync("channel_name", 1, new List<Log>()).RunNotAsync());
        }
    }
}
