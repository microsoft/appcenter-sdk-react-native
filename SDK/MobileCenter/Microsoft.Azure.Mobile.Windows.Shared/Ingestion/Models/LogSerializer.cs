using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public static class LogSerializer
    {
        private static readonly LogJsonConverter Converter = new LogJsonConverter();
        private static readonly JsonSerializerSettings SerializationSettings;
        private static readonly JsonSerializerSettings DeserializationSettings;
        internal const string TypeIdKey = "type";

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
                        new Rest.Serialization.PolymorphicSerializeJsonConverter<Log>(TypeIdKey)
        
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

        public static void AddLogType(string typeName, Type logType)
        {
            Converter.AddLogType(typeName, logType);
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
    }
}
