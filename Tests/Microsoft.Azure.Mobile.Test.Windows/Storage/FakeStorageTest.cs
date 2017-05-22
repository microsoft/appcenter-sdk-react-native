using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
                .Callback(() => Task.Delay(Timeout.InfiniteTimeSpan).Wait())
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            var countdownEvent = new CountdownEvent(2);
            Func<Task> putTask = () =>
            {
                countdownEvent.Signal();
                return storage.PutLogAsync(StorageTestChannelName, new TestLog());
            };
            Task.Run(putTask);
            Task.Run(putTask);
            
            // Wait for tasks started and bit more.
            countdownEvent.Wait();
            Task.Delay(100).Wait();

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
                    c => c.InsertAsync(It.IsAny<Mobile.Storage.Storage.LogEntry>()))
                .Callback(() => Task.Delay(TimeSpan.FromSeconds(2)))
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            Task.Run(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Run(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
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
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            var result = storage.ShutdownAsync(TimeSpan.FromSeconds(10)).RunNotAsync();
            Assert.IsTrue(result);
            Assert.ThrowsException<StatefulMutexException>(
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
