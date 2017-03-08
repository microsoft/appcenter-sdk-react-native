using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Xunit;

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;

    public class StorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";
        private readonly Mobile.Storage.Storage _storage = new Mobile.Storage.Storage();

        public void InitializeStorageTest()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
        }

        /// <summary>
        /// Verify that counting number of logs stored when there are no logs returns 0.
        /// </summary>
        [Fact]
        public void CountEmptyStorage()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.Equal(0, count);
        }

        /// <summary>
        /// Verify that after adding 'n' logs, counting logs returns 'n'.
        /// </summary>
        [Fact]
        public void CountNonemptyStorage()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            PutNLogs(numLogsToAdd);
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.Equal(numLogsToAdd, count);
        }

        /// <summary>
        /// Verify that storing a log and then retrieving it from storage does not alter the log.
        /// </summary>
        [Fact]
        public void PutOneLog()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var addedLog = TestLog.CreateTestLog();
            _storage.PutLogAsync(StorageTestChannelName, addedLog).RunNotAsync();
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).RunNotAsync();
            var retrievedLog = retrievedLogs[0];
            Assert.Equal(addedLog, retrievedLog);
        }

        /// <summary>
        /// Verify that deleting all logs for a given channel does so.
        /// </summary>
        [Fact]
        public void DeleteLogsNoBatchId()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            PutNLogs(5);
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.Equal(0, count);
        }

        /// <summary>
        /// Verify that deleting a particular batch deletes exactly the number of logs for that batch.
        /// </summary>
        [Fact]
        public void DeleteLogsWithBatchId()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = 3;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            var batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            _storage.DeleteLogsAsync(StorageTestChannelName, batchId).RunNotAsync();
            var numLogsRemaining = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.Equal(numLogsToAdd - retrievedLogs.Count, numLogsRemaining);
        }

        /// <summary>
        /// Verify that when the limit equals the number of logs for the given channel, all logs are returned.
        /// </summary>
        [Fact]
        public void GetLogsExactLimit()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.True(IsSubset(addedLogs, retrievedLogs));
            Assert.True(IsSubset(retrievedLogs, addedLogs));
        }

        /// <summary>
        /// Verify that when the limit is lower than the number of logs for the given channel, all logs are returned.
        /// </summary>
        [Fact]
        public void GetLogsLowLimit()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = 3;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.Equal(limit, retrievedLogs.Count);
            Assert.True(IsSubset(addedLogs, retrievedLogs));
        }

        /// <summary>
        /// Verify that when the limit exceeds the number of logs for the given channel, 'limit' logs are correctly returned.
        /// </summary>
        [Fact]
        public void GetLogsHighLimit()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = 7;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.Equal(retrievedLogs, addedLogs);
        }

        /// <summary>
        /// Verify that when logs are retrieved, the batchId is not null.
        /// </summary>
        [Fact]
        public void GetLogsHasBatchId()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            string batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.NotNull(batchId);
        }

        /// <summary>
        /// Verify that when no logs are retrieved, the batchId is null.
        /// </summary>
        [Fact]
        public void GetNoLogsHasNoBatchId()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 0;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            var batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.Null(batchId);
        }

        /// <summary>
        /// Verify that storage does not return same log more than once.
        /// </summary>
        [Fact]
        public void GetDuplicateLogs()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogsFirstTry = new List<Log>();
            var retrievedLogsSecondTry = new List<Log>();

            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsFirstTry).RunNotAsync();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsSecondTry).RunNotAsync();

            Assert.Equal(addedLogs, retrievedLogsFirstTry);
            Assert.Equal(0, retrievedLogsSecondTry.Count);
        }

        /// <summary>
        /// Verify that a channel that starts with the name of another channel does not cause problems.
        /// </summary>
        [Fact]
        public void GetLogsFromChannelWithSimilarNames()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var fakeChannelName = StorageTestChannelName.Substring(0, StorageTestChannelName.Length - 1);
            _storage.PutLogAsync(StorageTestChannelName, TestLog.CreateTestLog()).RunNotAsync();
            var retrievedLogs = new List<Log>();
            var batchId = _storage.GetLogsAsync(fakeChannelName, 1, retrievedLogs).RunNotAsync();
            Assert.Null(batchId);
        }

        /// <summary>
        /// Verify that storage returns log more than once if pending state is cleared.
        /// </summary>
        [Fact]
        public void ClearPendingState()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);

            var retrievedLogsFirstTry = new List<Log>();
            var retrievedLogsSecondTry = new List<Log>();

            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsFirstTry).RunNotAsync();
            _storage.ClearPendingLogStateAsync(StorageTestChannelName).RunNotAsync();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsSecondTry).RunNotAsync();

            Assert.Equal(addedLogs, retrievedLogsFirstTry);
            Assert.Equal(addedLogs, retrievedLogsSecondTry);
        }

        /// <summary>
        /// Verify that an invalid log in the database, when retrieved, is deleted and no logs are returned.
        /// </summary>
        [Fact]
        public void FailToGetALog()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            StorageAdapter storageAdapter = new StorageAdapter("Microsoft.Azure.Mobile.Storage");
            storageAdapter.OpenAsync().RunNotAsync();
            var command = storageAdapter.CreateCommand();
            var logJsonString = "'this is not a valid log json string'";
            var channelParameter = command.CreateParameter();
            channelParameter.ParameterName = "channelName";
            channelParameter.Value = StorageTestChannelName;
            var logParameter = command.CreateParameter();
            logParameter.ParameterName = "log";
            logParameter.Value = logJsonString;
            command.Parameters.Add(channelParameter);
            command.Parameters.Add(logParameter);
            command.CommandText = "INSERT INTO logs (channel, log) " +
                                  $"VALUES (@{channelParameter.ParameterName}, @{logParameter.ParameterName})";
            command.Prepare();
            command.ExecuteNonQuery();
            storageAdapter.Close();
            var logs = new List<Log>();

            var batchId = _storage.GetLogsAsync(StorageTestChannelName, 4, logs).RunNotAsync();
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();

            Assert.Null(batchId);
            Assert.Equal(0, logs.Count);
            Assert.Equal(0, count);
        }

        #region Helper methods
        private List<TestLog> PutNLogs(int n)
        {
            Task[] putLogTasks = new Task[n];
            List<TestLog> addedLogs = new List<TestLog>();
            for (int i = 0; i < n; ++i)
            {
                var testLog = TestLog.CreateTestLog();
                addedLogs.Add(testLog);
                putLogTasks[i] = _storage.PutLogAsync(StorageTestChannelName, testLog);
            }
            Task.WaitAll(putLogTasks);
            return addedLogs;
        }

        private bool IsSubset(IEnumerable<Log> set, IEnumerable<Log> subset)
        {
            return set == subset || subset.All(log => set.Contains(log));
        }
        #endregion
    }
}
