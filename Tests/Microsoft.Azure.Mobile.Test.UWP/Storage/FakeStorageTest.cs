using System.Data.Common;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using HyperMock;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Data.Sqlite;

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;
    using Storage = Storage.Storage;

    [TestClass]
    public class FakeStorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";

        private class ConcreteDbException : DbException
        {
        }

        /// <summary>
        /// Verify that if an error occurs while executing a query, a StorageException gets thrown.
        /// </summary>
        [TestMethod]
        public void GetLogsQueryError()
        {
            var mockAdapter = Mock.Create<IStorageAdapter>();
            mockAdapter.Setup(adapter => adapter.OpenAsync()).Returns(GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteNonQueryAsync(Param.IsAny<DbCommand>())).Returns(GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteQueryAsync(Param.IsAny<DbCommand>())).Throws(new ConcreteDbException());

            /* Return SqliteCommand because DbCommand is abstract */
            mockAdapter.Setup(adapter => adapter.CreateCommand()).Returns(new SqliteCommand());

            var fakeStorage = new Storage(mockAdapter.Object);

            var logs = new List<Log>();
            Assert.ThrowsException<StorageException>(() =>
                fakeStorage.GetLogsAsync(StorageTestChannelName, 1, logs).RunNotAsync());
        }

        #region Helper methods

        static Task GetCompletedTask()
        {
            Task completedTask = Task.Delay(0);
            completedTask.Wait();
            return completedTask;
        }

        #endregion
    }
}
