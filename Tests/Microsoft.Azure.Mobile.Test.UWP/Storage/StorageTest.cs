using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test
{
    using System.Collections.Generic;
    using Storage = Storage.Storage;

    [TestClass]
    public class StorageTest
    {
        string StorageTestChannelName = "storageTestChannelName";

        [TestMethod]
        public void TestCreateStorage()
        {
            Storage storage = new Storage();
            int count = storage.CountLogsAsync(StorageTestChannelName).RunNotAsync();
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void TestPutLog()
        {
            LogSerializer.AddFactory(TestLog.JsonIdentifier, new LogFactory<TestLog>());
            Storage storage = new Storage();
            TestLog log = new TestLog();
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties["hello"] = "goodbye";
            properties["up"] = "down";
            log.Properties = properties;
            storage.PutLogAsync(StorageTestChannelName, log).RunNotAsync();
            List<Log> retrievedLogs = new List<Log>();
            string batchId = storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).RunNotAsync();

            Assert.IsTrue(retrievedLogs.Count == 1);
            TestLog retrievedLog = retrievedLogs[0] as TestLog;
            Assert.IsNotNull(retrievedLog);
            Assert.IsTrue(Utils.StringDictionariesAreEqual(log.Properties, retrievedLog.Properties));
        }
    }
}
