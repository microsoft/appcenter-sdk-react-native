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
        private static readonly LogJsonConverter Converter = new LogJsonConverter();
        private static readonly JsonSerializerSettings SerializationSettings;
        private static readonly JsonSerializerSettings DeserializationSettings;
        static LogSerializer()
        {
            SerializationSettings = new JsonSerializerSettings
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
            DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Rest.Serialization.Iso8601TimeSpanConverter(),
                        Converter
                    }
            };
        }

        public static void AddFactory(string typeName, ILogFactory factory)
        {
            Converter.AddFactory(typeName, factory);
        }
        public static string Serialize(LogContainer logContainer)
        {
            return Rest.Serialization.SafeJsonConvert.SerializeObject(logContainer, SerializationSettings);
        }
        public static string Serialize(Log log)
        {
            return Rest.Serialization.SafeJsonConvert.SerializeObject(log, SerializationSettings);
        }

        public static Log DeserializeLog(string json)
        {
            return Rest.Serialization.SafeJsonConvert.DeserializeObject<Log>(json, DeserializationSettings);
        }
        public static LogContainer DeserializeContainer(string json)
        {
            return Rest.Serialization.SafeJsonConvert.DeserializeObject<LogContainer>(json, DeserializationSettings);
        }
    }
}
