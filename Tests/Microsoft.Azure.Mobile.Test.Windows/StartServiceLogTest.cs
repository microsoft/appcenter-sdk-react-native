using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Test.Windows
{
    [TestClass]
    public class StartServiceLogTest
    {
        private const string StorageTestChannelName = "storageTestChannelName";

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
            StartServiceLog log = new StartServiceLog();
            Assert.IsNull(log.Device);
            Assert.IsNull(log.Services);
            Assert.IsNull(log.Sid);
            Assert.AreEqual(0, log.Toffset);
        }

        /// <summary>
        /// Validate that name services can be correctly saved and restored
        /// </summary>
        [TestMethod]
        public void SaveStartServiceLog()
        {
            StartServiceLog addedLog = new StartServiceLog();
            addedLog.Device = new DeviceInformationHelper().GetDeviceInformation();
            addedLog.Toffset = TimeHelper.CurrentTimeInMilliseconds();
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

        /// <summary>
        /// Validate that log is not valid with nullable 'Services'
        /// </summary>
        [TestMethod]
        public void ValidateStartServiceLog()
        {
            StartServiceLog log = new StartServiceLog();
            log.Device = new DeviceInformationHelper().GetDeviceInformation();
            log.Toffset = TimeHelper.CurrentTimeInMilliseconds();

            Assert.ThrowsException<Rest.ValidationException>( (Action)log.Validate );
        }
    }
}
