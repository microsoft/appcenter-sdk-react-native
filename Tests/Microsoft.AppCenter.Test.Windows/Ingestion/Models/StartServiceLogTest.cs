using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    [TestClass]
    public class StartServiceLogTest
    {
        private const string StorageTestChannelName = "startServiceStorageTestChannelName";

        [TestInitialize]
        public void InitializeStartServiceTest()
        {
            LogSerializer.AddLogType(StartServiceLog.JsonIdentifier, typeof(StartServiceLog));
        }

        /// <summary>
        /// Verify that default values are null (or 0 in offset case)
        /// </summary>
        [TestMethod]
        public void CheckInitialValues()
        {
            var log = new StartServiceLog();
            Assert.IsNull(log.Device);
            Assert.AreEqual(0, log.Services.Count);
            Assert.IsNull(log.Sid);
            Assert.AreEqual(null, log.Timestamp);
        }

        /// <summary>
        /// Validate that services names are coping
        /// </summary>
        [TestMethod]
        public void CheckInitialValuesWithServices()
        {
            var servicesNames = new List<string> { "Service0", "Service1", "Service2" };
            var log = new StartServiceLog();
            log.Services = servicesNames;
            Assert.IsNotNull(log.Services);
            foreach (var serviceName in log.Services)
            {
                Assert.IsTrue(servicesNames.Contains(serviceName));
            }
        }

        /// <summary>
        /// Validate that name services can be correctly saved and restored
        /// </summary>
        [TestMethod]
        public void SaveStartServiceLog()
        {
            var addedLog = new StartServiceLog
            {
                Device = new DeviceInformationHelper().GetDeviceInformationAsync().RunNotAsync(),
                Timestamp = DateTime.Now,
                Services = new List<string> {"Service0", "Service1", "Service2"},
                Sid = Guid.NewGuid()
            };

            var storage = new Microsoft.AppCenter.Storage.Storage();
            storage.DeleteLogs(StorageTestChannelName);
            storage.PutLog(StorageTestChannelName, addedLog);
            var retrievedLogs = new List<Log>();
            storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).RunNotAsync();
            var retrievedLog = retrievedLogs[0] as StartServiceLog;

            foreach (var serviceName in addedLog.Services)
            {
                Assert.IsTrue(retrievedLog.Services.Contains(serviceName));
            }
        }
    }
}
