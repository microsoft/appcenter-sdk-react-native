using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Test.Storage;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite;

namespace Microsoft.Azure.Mobile.Test
{

    [TestClass]
    public class FakeStorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";

        /// <summary>
        /// Verify that shutdown fails when tasks exceed time limit
        /// </summary>
        [TestMethod]
        public void ShutdownTimeout()
        {
            var mockConnection = new Mock<SQLiteAsyncConnection>("tbl", true);
            mockConnection.Setup(
                    c => c.InsertAsync(It.IsAny<Mobile.Storage.Storage.LogEntry>()))
                .Callback(() => Task.Delay(TimeSpan.FromSeconds(2)))
                .Returns(TaskExtension.GetCompletedTask(1));
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            Task.Factory.StartNew(() => storage.PutLogAsync(StorageTestChannelName, new TestLog()));
            var result = storage.Shutdown(TimeSpan.FromMilliseconds(1));

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verify that shutdown is successful when tasks complete in time
        /// </summary>
        [TestMethod]
        public void ShutdownSucceed()
        {
            var mockConnection = new Mock<SQLiteAsyncConnection>();
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
            var mockConnection = new Mock<SQLiteAsyncConnection>();
            var storage = new Mobile.Storage.Storage(mockConnection.Object);
            storage.Shutdown(TimeSpan.FromSeconds(10));
            Assert.ThrowsException<StorageException>(
                () => storage.GetLogsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<Log>>()));
        }

        /// <summary>
        /// Verify that if an error occurs while executing a query (from GetLogs), a StorageException gets thrown.
        /// </summary>
        [TestMethod]
        public void GetLogsQueryError()
        {
            var mockConnection = new Mock<SQLiteAsyncConnection>();
            mockConnection.Setup(
                    c => c.Table<Mobile.Storage.Storage.LogEntry>())
                .Throws(new TestSqliteException());
            var fakeStorage = new Mobile.Storage.Storage(mockConnection.Object);
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
            var mockConnection = new Mock<SQLiteAsyncConnection>();
            mockConnection.Setup(
                    c => c.Table<Mobile.Storage.Storage.LogEntry>())
                .Throws(new TestSqliteException());
            mockConnection.Setup(c => c.InsertAsync(It.IsAny<Mobile.Storage.Storage.LogEntry>()))
                .Throws(new TestSqliteException());


            var fakeStorage = new Mobile.Storage.Storage(mockConnection.Object);
            try
            {
                fakeStorage.PutLogAsync("channel_name", new TestLog()).RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e as StorageException);
            }

            try
            {
                fakeStorage.PutLogAsync("channel_name", new TestLog()).RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e as StorageException);
            }

            try
            {
                fakeStorage.DeleteLogsAsync("channel_name", string.Empty).RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e as StorageException);
            }

            try
            {
                fakeStorage.DeleteLogsAsync("channel_name").RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e as StorageException);
            }

            try
            {
                fakeStorage.CountLogsAsync("channel_name").RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e as StorageException);
            }

            try
            {
                fakeStorage.GetLogsAsync("channel_name", 1, new List<Log>()).RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e as StorageException);
            }
        }
    }
}
