using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;

    [TestClass]
    public class StorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";
        private readonly Mobile.Storage.Storage _storage = new Mobile.Storage.Storage();

        [TestInitialize]
        public void InitializeStorageTest()
        {
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
        }

        /// <summary>
        /// Verify that counting number of logs stored when there are no logs returns 0.
        /// </summary>
        [TestMethod]
        public void CountEmptyStorage()
        {
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(0, count);
        }

        /// <summary>
        /// Verify that after adding 'n' logs, counting logs returns 'n'.
        /// </summary>
        [TestMethod]
        public void CountNonemptyStorage()
        {
            var numLogsToAdd = 5;
            PutNLogs(numLogsToAdd);
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(numLogsToAdd, count);
        }

        /// <summary>
        /// Verify that storing a log and then retrieving it from storage does not alter the log.
        /// </summary>
        [TestMethod]
        public void PutOneLog()
        {
            var addedLog = TestLog.CreateTestLog();
            _storage.PutLogAsync(StorageTestChannelName, addedLog).RunNotAsync();
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).RunNotAsync();
            var retrievedLog = retrievedLogs[0];
            Assert.AreEqual(addedLog, retrievedLog);
        }

        /// <summary>
        /// Verify that deleting all logs for a given channel does so.
        /// </summary>
        [TestMethod]
        public void DeleteLogsNoBatchId()
        {
            PutNLogs(5);
            _storage.DeleteLogsAsync(StorageTestChannelName).RunNotAsync();
            var count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(0, count);
        }

        /// <summary>
        /// Verify that deleting a particular batch deletes exactly the number of logs for that batch.
        /// </summary>
        [TestMethod]
        public void DeleteLogsWithBatchId()
        {
            var numLogsToAdd = 5;
            var limit = 3;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            var batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            _storage.DeleteLogsAsync(StorageTestChannelName, batchId).RunNotAsync();
            var numLogsRemaining = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(numLogsToAdd - retrievedLogs.Count, numLogsRemaining);
        }

        /// <summary>
        /// Verify that when the limit equals the number of logs for the given channel, all logs are returned.
        /// </summary>
        [TestMethod]
        public void GetLogsExactLimit()
        {
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            CollectionAssert.AreEquivalent(addedLogs, retrievedLogs);
        }

        /// <summary>
        /// Verify that when the limit is lower than the number of logs for the given channel, all logs are returned.
        /// </summary>
        [TestMethod]
        public void GetLogsLowLimit()
        {
            var numLogsToAdd = 5;
            var limit = 3;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.AreEqual(limit, retrievedLogs.Count);
            CollectionAssert.IsSubsetOf(retrievedLogs, addedLogs);
        }

        /// <summary>
        /// Verify that when the limit exceeds the number of logs for the given channel, 'limit' logs are correctly returned.
        /// </summary>
        [TestMethod]
        public void GetLogsHighLimit()
        {
            var numLogsToAdd = 5;
            var limit = 7;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            CollectionAssert.AreEqual(retrievedLogs, addedLogs);
        }

        /// <summary>
        /// Verify that when logs are retrieved, the batchId is not null.
        /// </summary>
        [TestMethod]
        public void GetLogsHasBatchId()
        {
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            string batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.IsNotNull(batchId);
        }

        /// <summary>
        /// Verify that when no logs are retrieved, the batchId is null.
        /// </summary>
        [TestMethod]
        public void GetNoLogsHasNoBatchId()
        {
            var numLogsToAdd = 0;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogs = new List<Log>();
            var batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.IsNull(batchId);
        }

        /// <summary>
        /// Verify that storage does not return same log more than once.
        /// </summary>
        [TestMethod]
        public void GetDuplicateLogs()
        {
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            var retrievedLogsFirstTry = new List<Log>();
            var retrievedLogsSecondTry = new List<Log>();

            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsFirstTry).RunNotAsync();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsSecondTry).RunNotAsync();

            CollectionAssert.AreEqual(addedLogs, retrievedLogsFirstTry);
            Assert.AreEqual(0, retrievedLogsSecondTry.Count);
        }

        /// <summary>
        /// Verify that a channel that starts with the name of another channel does not cause problems.
        /// </summary>
        [TestMethod]
        public void GetLogsFromChannelWithSimilarNames()
        {
            var fakeChannelName = StorageTestChannelName.Substring(0, StorageTestChannelName.Length - 1);
            _storage.PutLogAsync(StorageTestChannelName, TestLog.CreateTestLog()).RunNotAsync();
            var retrievedLogs = new List<Log>();
            var batchId = _storage.GetLogsAsync(fakeChannelName, 1, retrievedLogs).RunNotAsync();
            Assert.IsNull(batchId);
        }
       
        /// <summary>
        /// Verify that storage returns log more than once if pending state is cleared.
        /// </summary>
        [TestMethod]
        public void ClearPendingState()
        {
            var numLogsToAdd = 5;
            var limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);

            var retrievedLogsFirstTry = new List<Log>();
            var retrievedLogsSecondTry = new List<Log>();

            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsFirstTry).RunNotAsync();
            _storage.ClearPendingLogStateAsync(StorageTestChannelName).RunNotAsync();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsSecondTry).RunNotAsync();

            CollectionAssert.AreEqual(addedLogs, retrievedLogsFirstTry);
            CollectionAssert.AreEqual(addedLogs, retrievedLogsSecondTry);
        }

        /// <summary>
        /// Verify that an invalid log in the database, when retrieved, is deleted and no logs are returned.
        /// </summary>
        [TestMethod]
        public void FailToGetALog()
        {
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

            Assert.IsNull(batchId);
            Assert.AreEqual(0, logs.Count);
            Assert.AreEqual(0, count);
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
        #endregion
    }
}
