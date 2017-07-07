using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models.Serialization;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    [TestClass]
    public class CustomPropertiesLogTest
    {
        private const string StorageTestChannelName = "customPropertiesStorageTestChannelName";

        [TestInitialize]
        public void InitializeStartServiceTest()
        {
            LogSerializer.AddLogType(CustomPropertiesLog.JsonIdentifier, typeof(CustomPropertiesLog));
        }

        /// <summary>
        /// Verify that default values are null (or 0 in offset case)
        /// </summary>
        [TestMethod]
        public void CheckInitialValues()
        {
            var log = new CustomPropertiesLog();
            Assert.IsNull(log.Device);
            Assert.AreEqual(0, log.Properties.Count);
            Assert.IsNull(log.Sid);
            Assert.AreEqual(null, log.Timestamp);
        }

        /// <summary>
        /// Validate that properties can be correctly saved and restored
        /// </summary>
        [TestMethod]
        public void SaveCustomPropertiesLog()
        {
            var addedLog = new CustomPropertiesLog
            {
                Device = new DeviceInformationHelper().GetDeviceInformationAsync().RunNotAsync(),
                Timestamp = DateTime.Now,
                Properties = new Dictionary<string, object>
                {
                    { "t1", "test" },
                    { "t2", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "t3", 1 },
                    { "t4", 0.1f },
                    { "t5", false },
                    { "t6", null }
                },
                Sid = Guid.NewGuid()
            };

            var storage = new Mobile.Storage.Storage();
            storage.DeleteLogs(StorageTestChannelName);
            storage.PutLog(StorageTestChannelName, addedLog);
            var retrievedLogs = new List<Log>();
            storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).RunNotAsync();
            var retrievedLog = retrievedLogs[0] as CustomPropertiesLog;

            foreach (var addedProperty in addedLog.Properties)
            {
                object retrievedProperty;
                Assert.IsTrue(retrievedLog.Properties.TryGetValue(addedProperty.Key, out retrievedProperty));
                Assert.IsTrue(EqualityComparer<object>.Default.Equals(addedProperty.Value, retrievedProperty));
            }
        }

        /// <summary>
        /// Validate that log is not valid with nullable 'Properties'
        /// </summary>
        [TestMethod]
        public void ValidateStartServiceLog()
        {
            var log = new CustomPropertiesLog
            {
                Properties = null,
                Device = new DeviceInformationHelper().GetDeviceInformationAsync().RunNotAsync(),
                Timestamp = DateTime.Now
            };

            Assert.ThrowsException<ValidationException>((Action)log.Validate);
        }
    }
}
