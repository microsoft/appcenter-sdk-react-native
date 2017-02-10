using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    //TODO thread safety?
    public static class LogSerializer
    {
        private static LogJsonConverter _converter = new LogJsonConverter();
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
                        new Rest.Serialization.Iso8601TimeSpanConverter(),
                        new Rest.Serialization.PolymorphicSerializeJsonConverter<Log>("type")
        
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
                        new Rest.Serialization.Iso8601TimeSpanConverter(),
                        _converter
                    }
            };
        }

        public static void AddFactory(string typeName, ILogFactory factory)
        {
            _converter.AddFactory(typeName, factory);
        }
        public static string Serialize(LogContainer logContainer)
        {
            return Rest.Serialization.SafeJsonConvert.SerializeObject(logContainer, _serializationSettings);
        }
        public static string Serialize(Log log)
        {
            return Rest.Serialization.SafeJsonConvert.SerializeObject(log, _serializationSettings);
        }

        public static Log DeserializeLog(string json)
        {
            return Rest.Serialization.SafeJsonConvert.DeserializeObject<Log>(json, _deserializationSettings);
        }
        public static LogContainer DeserializeContainer(string json)
        {
            return Rest.Serialization.SafeJsonConvert.DeserializeObject<LogContainer>(json, _deserializationSettings);
        }
    }
}
