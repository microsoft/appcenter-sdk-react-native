using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    [TestClass]
    public class CustomPropertiesLogTest
    {
        private const string StorageTestChannelName = "customPropertyStorageTestChannelName";

        [TestInitialize]
        public void InitializeStartServiceTest()
        {
            LogSerializer.AddLogType(CustomPropertyLog.JsonIdentifier, typeof(CustomPropertyLog));
        }

        /// <summary>
        /// Verify that default values are null (or 0 in offset case)
        /// </summary>
        [TestMethod]
        public void CheckInitialValues()
        {
            var log = new CustomPropertyLog();
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
            var addedLog = new CustomPropertyLog
            {
                Device = new DeviceInformationHelper().GetDeviceInformationAsync().GetAwaiter().GetResult(),
                Timestamp = DateTime.Now,
                Properties = new List<CustomProperty>
                {
                    new StringProperty("t1", "test"),
                    new DateTimeProperty("t2",  new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    new NumberProperty("t3", (long)1),
                    new NumberProperty("t4", 0.1),
                    new BooleanProperty("t5", false),
                    new ClearProperty("t6")
                },
                Sid = Guid.NewGuid()
            };

            var storage = new Microsoft.AppCenter.Storage.Storage();
            storage.DeleteLogs(StorageTestChannelName);
            storage.PutLog(StorageTestChannelName, addedLog);
            var retrievedLogs = new List<Log>();
            storage.GetLogsAsync(StorageTestChannelName, 1, retrievedLogs).GetAwaiter().GetResult();
            var retrievedLog = retrievedLogs[0] as CustomPropertyLog;

            foreach (var addedProperty in addedLog.Properties)
            {
                var retrievedProperty = GetPropertyWithName(retrievedLog.Properties, addedProperty.Name);
                Assert.IsNotNull(retrievedProperty);
                Assert.AreEqual(addedProperty.GetValue(), retrievedProperty.GetValue());
            }
        }

        private static CustomProperty GetPropertyWithName(IList<CustomProperty> properties, string name)
        {
            foreach (var property in properties)
            {
                if (property.Name == name)
                {
                    return property;
                }
            }
            return null;
        }
    }
}
