using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Test.Windows
{
    [TestClass]
    public class StartServiceLogTests
    {
        private const string StorageTestChannelName = "storageTestChannelName";

        [TestInitialize]
        public void asdf()
        {
            LogSerializer.AddFactory(StartServiceLog.JsonIdentifier, new LogFactory<StartServiceLog>());
        }

        [TestMethod]
        public void CheckInitialValues()
        {
            StartServiceLog log = new StartServiceLog();
            Assert.IsNull(log.Device);
            Assert.IsNull(log.Services);
            Assert.IsNull(log.Sid);
            Assert.AreEqual(0, log.Toffset);
        }

        [TestMethod]
        public void SaveStartServiceLog()
        {
            StartServiceLog addedLog = new StartServiceLog();
            addedLog.Device = new DeviceInformationHelper().GetDeviceInformation();
            addedLog.Toffset = TimeHelper.CurrentTimeInMilliseconds();

            Assert.ThrowsException<Rest.ValidationException>((Action)addedLog.Validate);

            addedLog.Services = new List<string>() { "Service0", "Service1", "Service2" };

            Mobile.Storage.Storage _storage = new Mobile.Storage.Storage();
            _storage.PutLogAsync(StorageTestChannelName, addedLog).RunNotAsync();
            var retrievedLogs = new List<Log>();
            _storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).RunNotAsync();
            StartServiceLog retrievedLog = retrievedLogs[0] as StartServiceLog;

            foreach (string serviceName in addedLog.Services)
            {
                Assert.IsTrue(retrievedLog.Services.Contains(serviceName));
            }
        }
    }
}
