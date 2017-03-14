using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Rest.Serialization;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public class LogJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _logTypes = new Dictionary<string, Type>();
        private readonly PolymorphicDeserializeJsonConverter<Log> _converter = new PolymorphicDeserializeJsonConverter<Log>(LogSerializer.TypeIdKey);
        private readonly object _jsonConverterLock = new object();

        public void AddLogType(string typeName, Type type)
        {
            lock (_jsonConverterLock)
            {
                _logTypes[typeName] = type;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return _converter.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            lock (_jsonConverterLock)
            {
                var jsonObject = JObject.Load(reader);
                var typeName = jsonObject.GetValue(LogSerializer.TypeIdKey)?.ToString();
                if (typeName == null || !_logTypes.ContainsKey(typeName))
                {
                    throw new JsonReaderException("Could not identify type of log");
                }
                return _converter.ReadJson(jsonObject.CreateReader(), _logTypes[typeName], existingValue, serializer);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            /* No-op */
        }
    }
}
