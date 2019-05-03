// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Ingestion.Models.Serialization
{
    public class LogJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _logTypes = new Dictionary<string, Type>();
        private readonly object _jsonConverterLock = new object();
        private static readonly JsonSerializerSettings SerializationSettings;

        internal const string TypeIdKey = "type";

        static LogJsonConverter()
        {
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Converters = { new CustomPropertyJsonConverter() }
            };
        }

        public void AddLogType(string typeName, Type type)
        {
            lock (_jsonConverterLock)
            {
                _logTypes[typeName] = type;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Log).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type logType;
            var jsonObject = JObject.Load(reader);
            var typeName = jsonObject.GetValue(TypeIdKey)?.ToString();
            lock (_jsonConverterLock)
            {
                if (typeName == null || !_logTypes.ContainsKey(typeName))
                {
                    throw new JsonReaderException("Could not identify type of log");
                }
                logType = _logTypes[typeName];
                jsonObject.Remove(TypeIdKey);
                if (logType == typeof(CustomPropertyLog))
                {
                    return ReadCustomPropertyLog(jsonObject);
                }
            }
            return jsonObject.ToObject(logType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var info = value.GetType().GetTypeInfo();
            var attribute = info.GetCustomAttribute(typeof(JsonObjectAttribute)) as JsonObjectAttribute;
            if (attribute == null)
            {
                throw new JsonWriterException("Cannot serialize log; Log type is missing JsonObjectAttribute");
            }
            var jsonText = JsonConvert.SerializeObject(value, SerializationSettings);
            var jsonObject = JObject.Parse(jsonText);
            jsonObject.Add(TypeIdKey, JToken.FromObject(attribute.Id));
            writer.WriteRawValue(jsonObject.ToString());
        }

        public Log ReadCustomPropertyLog(JObject logObject)
        {
            var propertiesIdentifier = "properties";
            var propertiesJson = logObject.GetValue(propertiesIdentifier);
            logObject.Remove(propertiesIdentifier);
            var customPropertiesLog = logObject.ToObject(typeof(CustomPropertyLog)) as CustomPropertyLog;
            foreach (var child in propertiesJson.Children())
            {
                var propertyJson = child.ToString();
                var property = JsonConvert.DeserializeObject<CustomProperty>(propertyJson, SerializationSettings);
                customPropertiesLog.Properties.Add(property);
            }
            return customPropertiesLog;
        }
    }
}
