using System.Data.Common;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Moq;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;

    public class FakeStorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";

        private class ConcreteDbException : DbException
        {
        }

        /// <summary>
        /// Verify that if an error occurs while executing a query (from GetLogs), a StorageException gets thrown.
        /// </summary>
        [Fact]
        public void GetLogsQueryError()
        {
            var mockAdapter = new Mock<IStorageAdapter>();
            mockAdapter.Setup(adapter => adapter.OpenAsync()).Returns(TaskExtension.GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteNonQueryAsync(It.IsAny<DbCommand>())).Returns(TaskExtension.GetCompletedTask());
            mockAdapter.Setup(adapter => adapter.ExecuteQueryAsync(It.IsAny<DbCommand>())).Throws(new ConcreteDbException());

            /* Return SqliteCommand because DbCommand is abstract */
            mockAdapter.Setup(adapter => adapter.CreateCommand()).Returns(new SqliteCommand());

            var fakeStorage = new Mobile.Storage.Storage(mockAdapter.Object);

            var logs = new List<Log>();
            Assert.Throws<StorageException>(() =>
                fakeStorage.GetLogsAsync(StorageTestChannelName, 1, logs).RunNotAsync());
        }

    }
}
