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
        private Dictionary<string, ILogFactory> _logFactories = new Dictionary<string, ILogFactory>();
        private Microsoft.Rest.Serialization.PolymorphicDeserializeJsonConverter<Log> _converter = new Rest.Serialization.PolymorphicDeserializeJsonConverter<Log>("type");
        private object _jsonConverterLock = new object();
        public void AddFactory(string typeName, ILogFactory factory)
        {
            lock (_jsonConverterLock)
            {
                _logFactories[typeName] = factory;
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
            ILogFactory factory;
            lock (_jsonConverterLock)
            {
                factory = _logFactories[typeName];
            }
            var log = factory.Create();
            return _converter.ReadJson(jobject.CreateReader(), factory.LogType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            /* No-op */
        }
    }
}
