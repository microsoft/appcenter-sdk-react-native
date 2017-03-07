using System.Data.Common;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using HyperMock;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Data.Sqlite;

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;

    [TestClass]
    public class FakeStorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";

        private class ConcreteDbException : DbException
        {
        }

        /// <summary>
        /// Verify that if an error occurs while executing a query (from GetLogs), a StorageException gets thrown.
        /// </summary>
        [TestMethod]
        public void GetLogsQueryError()
        {
            var mockAdapter = Mock.Create<IStorageAdapter>();
            mockAdapter.Setup(adapter => adapter.OpenAsync()).Returns(TaskExtension.GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteNonQueryAsync(Param.IsAny<DbCommand>())).Returns(TaskExtension.GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteQueryAsync(Param.IsAny<DbCommand>())).Throws(new ConcreteDbException());

            /* Return SqliteCommand because DbCommand is abstract */
            mockAdapter.Setup(adapter => adapter.CreateCommand()).Returns(new SqliteCommand());

            var fakeStorage = new Mobile.Storage.Storage(mockAdapter.Object);

            var logs = new List<Log>();
            Assert.ThrowsException<StorageException>(() =>
                fakeStorage.GetLogsAsync(StorageTestChannelName, 1, logs).RunNotAsync());
        }

    }
}
