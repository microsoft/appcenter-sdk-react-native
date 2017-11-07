using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Ingestion.Models.Serialization
{
    public class CustomPropertyJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _customPropertyTypes = new Dictionary<string, Type>
        {
            { BooleanProperty.JsonIdentifier, typeof(BooleanProperty) },
            { ClearProperty.JsonIdentifier, typeof(ClearProperty) },
            { DateTimeProperty.JsonIdentifier, typeof(DateTimeProperty) },
            { NumberProperty.JsonIdentifier, typeof(NumberProperty) },
            { StringProperty.JsonIdentifier, typeof(StringProperty) }
        };

        private readonly object _jsonConverterLock = new object();
        private static readonly JsonSerializerSettings SerializationSettings;
        internal const string TypeIdKey = "type";

        public CustomPropertyJsonConverter()
        {
            _customPropertyTypes[BooleanProperty.JsonIdentifier] = typeof(BooleanProperty);
            _customPropertyTypes[ClearProperty.JsonIdentifier] = typeof(ClearProperty);
            _customPropertyTypes[ClearProperty.JsonIdentifier] = typeof(ClearProperty);
            _customPropertyTypes[ClearProperty.JsonIdentifier] = typeof(ClearProperty);

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(CustomProperty).IsAssignableFrom(objectType);
        }
    
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type logType;
            var jsonObject = JObject.Load(reader);
            var typeName = jsonObject.GetValue(TypeIdKey)?.ToString();
            lock (_jsonConverterLock)
            {
                if (typeName == null || !_customPropertyTypes.ContainsKey(typeName))
                {
                    throw new JsonReaderException("Could not identify type of log");
                }
                logType = _customPropertyTypes[typeName];
            }
            jsonObject.Remove(TypeIdKey);
            return jsonObject.ToObject(logType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var info = value.GetType().GetTypeInfo();
            var attribute = info.GetCustomAttribute(typeof(JsonObjectAttribute)) as JsonObjectAttribute;
            if (attribute == null)
            {
                throw new JsonWriterException("Cannot serialize property; Log type is missing JsonObjectAttribute");
            }
            var jsonText = JsonConvert.SerializeObject(value, SerializationSettings);
            var jsonObject = JObject.Parse(jsonText);
            jsonObject.Add(TypeIdKey, JToken.FromObject(attribute.Id));
            writer.WriteRawValue(jsonObject.ToString());
        }
    }
}
