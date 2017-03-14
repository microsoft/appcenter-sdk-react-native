using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{        
    //TODO thread safety? (investigate for polymorphicdeserializejsonconverter)

    public class LogJsonConverter : JsonConverter
    {
        private Dictionary<string, Type> _logTypes = new Dictionary<string, Type>();
        private Microsoft.Rest.Serialization.PolymorphicDeserializeJsonConverter<Log> _converter = new Rest.Serialization.PolymorphicDeserializeJsonConverter<Log>("type");
        private object _jsonConverterLock = new object();
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
            var jobject = JObject.Load(reader);
            string typeName = jobject["type"].ToString();
            Type logType;
            lock (_jsonConverterLock)
            {
                logType = _logTypes[typeName];
            }
            return _converter.ReadJson(jobject.CreateReader(), logType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            /* No-op */
        }
    }
}
