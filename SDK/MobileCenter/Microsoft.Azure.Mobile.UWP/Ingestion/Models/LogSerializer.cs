using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public static class LogSerializer
    {
        private static JsonSerializerSettings _serializationSettings;
        private static JsonSerializerSettings _deserializationSettings;
        static LogSerializer()
        {
            _serializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Rest.Serialization.Iso8601TimeSpanConverter()
                    }
            };
            _deserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Rest.Serialization.Iso8601TimeSpanConverter()
                    }
            };
            _serializationSettings.Converters.Add(new Rest.Serialization.PolymorphicSerializeJsonConverter<Log>("type"));
            _deserializationSettings.Converters.Add(new Rest.Serialization.PolymorphicDeserializeJsonConverter<Log>("type"));
        }

        public static string Serialize(LogContainer logContainer)
        {
            return SerializeItem(logContainer);
        }
        public static string Serialize(Log log)
        {
            return SerializeItem(log);
        }

        public static Log DeserializeLog(string json)
        {
            return Rest.Serialization.SafeJsonConvert.DeserializeObject<Log>(json, _deserializationSettings);
        }
        public static LogContainer DeserializeContainer(string json)
        {
            return Rest.Serialization.SafeJsonConvert.DeserializeObject<LogContainer>(json, _deserializationSettings);
        }

        private static string SerializeItem(object item)
        {
            return Rest.Serialization.SafeJsonConvert.SerializeObject(item, _serializationSettings);
        }

    }
}
