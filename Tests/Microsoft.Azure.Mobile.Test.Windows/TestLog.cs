using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Test
{
    [JsonObject(JsonIdentifier)]
    public class TestLog : LogWithProperties
    {
        internal const string JsonIdentifier = "testlog";

        static TestLog()
        {
            LogSerializer.AddLogType(JsonIdentifier, typeof(TestLog));
        }

        public static TestLog CreateTestLog()
        {
            var properties = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}, {"key3", "value3"}};
            return new TestLog(properties) {Sid = Guid.NewGuid()};
        }

        public TestLog()
        {
            Properties = new Dictionary<string, string>();
        }

        public TestLog(IDictionary<string, string> properties)
        {
            Properties = properties;
        }

        public override bool Equals(object obj)
        {
            var that = obj as TestLog;
            if (that == null)
            {
                return false;
            }

            var thisSerialized = LogSerializer.Serialize(this);
            var thatSerialized = LogSerializer.Serialize(that);
            return thisSerialized == thatSerialized;
        }

        public override int GetHashCode()
        {
            return LogSerializer.Serialize(this).GetHashCode();
        }
    }
}
