using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public class LogJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _logTypes = new Dictionary<string, Type>();
        private readonly object _jsonConverterLock = new object();
        private JsonSerializerSettings _serializationSettings;

        public void AddLogType(string typeName, Type type)
        {
            lock (_jsonConverterLock)
            {
                _logTypes[typeName] = type;
            }

            _serializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Log).IsAssignableFrom(objectType);
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
                jsonObject.Remove(LogSerializer.TypeIdKey);
                return JsonConvert.DeserializeObject(jsonObject.ToString(), _logTypes[typeName], _serializationSettings) as Log;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var info = value.GetType().GetTypeInfo();
            var attribute = info.GetCustomAttribute(typeof(JsonObjectAttribute)) as JsonObjectAttribute;
            if (attribute == null)
            {
                throw new JsonWriterException("Cannot deserialize log; missing JsonObjectAttribute");
            }
            
            var jsonText = JsonConvert.SerializeObject(value, _serializationSettings);
            var jsonObject = JObject.Parse(jsonText);
            jsonObject.Add(LogSerializer.TypeIdKey, JToken.FromObject(attribute.Id));
            writer.WriteRawValue(jsonObject.ToString());
        }
    }
}
