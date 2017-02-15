using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            LogSerializer.AddFactory(TestLog.JsonIdentifier, new LogFactory<TestLog>());
        }

        public static TestLog CreateTestLog()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("key1", "value1");
            properties.Add("key2", "value2");
            properties.Add("key3", "value3");
            TestLog log = new TestLog(properties);
            log.Sid = Guid.NewGuid();
            return new TestLog(properties);
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
            TestLog that = obj as TestLog;
            if (that == null)
            {
                return false;
            }

            string thisSerialized = LogSerializer.Serialize(this);
            string thatSerialized = LogSerializer.Serialize(that);
            return thisSerialized == thatSerialized;
        }

        public override int GetHashCode()
        {
            return LogSerializer.Serialize(this).GetHashCode();
        }
    }
}
