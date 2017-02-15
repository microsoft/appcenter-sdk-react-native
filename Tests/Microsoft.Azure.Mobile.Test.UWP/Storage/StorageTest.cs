using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
// ReSharper disable All

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;
    using Storage = Storage.Storage;

    [TestClass]
    public class StorageTest
    {
        const string StorageTestChannelName = "storageTestChannelName";

        Storage _storage = new Storage();

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
            int count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(0, count);
        }

        /// <summary>
        /// Verify that after adding 'n' logs, counting logs returns 'n'.
        /// </summary>
        [TestMethod]
        public void CountNonemptyStorage()
        {
            int numLogsToAdd = 5;
            PutNLogs(numLogsToAdd);
            int count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(numLogsToAdd, count);
        }

        /// <summary>
        /// Verify that storing a log and then retrieving it from storage does not alter the log.
        /// </summary>
        [TestMethod]
        public void PutOneLog()
        {
            TestLog addedLog = TestLog.CreateTestLog();
            _storage.PutLogAsync(StorageTestChannelName, addedLog).RunNotAsync();
            List<Log> retrievedLogs = new List<Log>();
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
            int count = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(0, count);
        }

        /// <summary>
        /// Verify that deleting a particular batch deletes exactly the number of logs for that batch.
        /// </summary>
        [TestMethod]
        public void DeleteLogsWithBatchId()
        {
            int numLogsToAdd = 5;
            int limit = 3;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            string batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            _storage.DeleteLogsAsync(StorageTestChannelName, batchId).RunNotAsync();
            int numLogsRemaining = _storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.AreEqual(numLogsToAdd - retrievedLogs.Count, numLogsRemaining);
        }

        /// <summary>
        /// Verify that when the limit equals the number of logs for the given channel, all logs are returned.
        /// </summary>
        [TestMethod]
        public void GetLogsExactLimit()
        {
            int numLogsToAdd = 5;
            int limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            CollectionAssert.AreEquivalent(addedLogs, retrievedLogs);
        }

        /// <summary>
        /// Verify that when the limit is lower than the number of logs for the given channel, all logs are returned.
        /// </summary>
        [TestMethod]
        public void GetLogsLowLimit()
        {
            int numLogsToAdd = 5;
            int limit = 3;
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
            int numLogsToAdd = 5;
            int limit = 7;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            CollectionAssert.AreEqual(retrievedLogs, addedLogs);
        }

        /// <summary>
        /// Verify that when logs are retrieved, the batchId is not null.
        /// </summary>
        [TestMethod]
        public void GetLogsHasBatchId()
        {
            int numLogsToAdd = 5;
            int limit = numLogsToAdd;
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
            int numLogsToAdd = 0;
            int limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);
            List<Log> retrievedLogs = new List<Log>();
            string batchId = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogs).RunNotAsync();
            Assert.IsNull(batchId);
        }

        /// <summary>
        /// Verify that storage does not return same log more than once.
        /// </summary>
        [TestMethod]
        public void GetDuplicateLogs()
        {
            int numLogsToAdd = 5;
            int limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);

            List<Log> retrievedLogsFirstTry = new List<Log>();
            List<Log> retrievedLogsSecondTry = new List<Log>();

            string batchIdFirst = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsFirstTry).RunNotAsync();
            string batchIdSecond = _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsSecondTry).RunNotAsync();

            CollectionAssert.AreEqual(addedLogs, retrievedLogsFirstTry);
            Assert.AreEqual(0, retrievedLogsSecondTry.Count);
        }

        /// <summary>
        /// Verify that a channel that starts with the name of another channel does not cause problems.
        /// </summary>
        [TestMethod]
        public void GetLogsFromChannelWithSimilarNames()
        {
            string fakeChannelName = StorageTestChannelName.Substring(0, StorageTestChannelName.Length - 1);
            _storage.PutLogAsync(StorageTestChannelName, TestLog.CreateTestLog()).RunNotAsync();
            List<Log> retrievedLogs = new List<Log>();
            string batchId = _storage.GetLogsAsync(fakeChannelName, 1, retrievedLogs).RunNotAsync();
            Assert.IsNull(batchId);
        }
       
        /// <summary>
        /// Verify that storage returns log more than once if pending state is cleared.
        /// </summary>
        [TestMethod]
        public void ClearPendingState()
        {
            int numLogsToAdd = 5;
            int limit = numLogsToAdd;
            var addedLogs = PutNLogs(numLogsToAdd);

            List<Log> retrievedLogsFirstTry = new List<Log>();
            List<Log> retrievedLogsSecondTry = new List<Log>();

            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsFirstTry).RunNotAsync();
            _storage.ClearPendingLogStateAsync(StorageTestChannelName).RunNotAsync();
            _storage.GetLogsAsync(StorageTestChannelName, limit, retrievedLogsSecondTry).RunNotAsync();

            CollectionAssert.AreEqual(addedLogs, retrievedLogsFirstTry);
            CollectionAssert.AreEqual(addedLogs, retrievedLogsSecondTry);
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
