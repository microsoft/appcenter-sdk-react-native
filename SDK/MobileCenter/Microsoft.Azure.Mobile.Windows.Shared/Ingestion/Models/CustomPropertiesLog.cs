using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    /// <summary>
    /// The custom properties log model.
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public class CustomPropertiesLog : Log
    {
        internal const string JsonIdentifier = "custom_properties";
        private const string PropertyType = "type";
        private const string PropertyName = "name";
        private const string PropertyValue = "value";
        private const string PropertyTypeClear = "clear";
        private const string PropertyTypeBoolean = "boolean";
        private const string PropertyTypeNumber = "number";
        private const string PropertyTypeDatetime = "date_time";
        private const string PropertyTypeString = "string";

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public CustomPropertiesLog()
        {
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Key/value pair properties.
        /// </summary>
        /// <remarks>JsonConverter attribute not supported here.</remarks>
        [JsonIgnore]
        public IDictionary<string, object> Properties { get; set; }

        [JsonProperty(PropertyName = "properties")]
        internal JArray JsonProperties
        {
            get { return WriteProperties(Properties); }
            set { Properties = ReadProperties(value); }
        }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (Properties == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(Properties));
            }
        }
        
        private static JArray WriteProperties(IDictionary<string, object> value)
        {
            if (value == null)
            {
                return null;
            }
            var properties = new JArray();
            foreach (var property in value)
            {
                properties.Add(WriteProperty(property.Key, property.Value));
            }
            return properties;
        }

        private static JObject WriteProperty(string key, object value)
        {
            var property = new JObject();
            property.Add(PropertyName, key);
            if (value == null)
            {
                property.Add(PropertyType, PropertyTypeClear);
            }
            else
            {
                var typeCode = Convert.GetTypeCode(value);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        property.Add(PropertyType, PropertyTypeBoolean);
                        property.Add(PropertyValue, JToken.FromObject(value));
                        break;
                    case TypeCode.Char:
                    case TypeCode.String:
                        property.Add(PropertyType, PropertyTypeString);
                        property.Add(PropertyValue, JToken.FromObject(value));
                        break;
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        property.Add(PropertyType, PropertyTypeNumber);
                        property.Add(PropertyValue, JToken.FromObject(value));
                        break;
                    case TypeCode.DateTime:
                        property.Add(PropertyType, PropertyTypeDatetime);
                        property.Add(PropertyValue, JToken.FromObject(value));
                        break;
                    default:
                        throw new JsonException("Invalid value type");
                }
            }
            return property;
        }

        private static IDictionary<string, object> ReadProperties(JArray value)
        {
            if (value == null)
            {
                return null;
            }
            var properties = new Dictionary<string, object>();
            foreach (var property in value.Children())
            {
                var pair = ReadProperty((JObject)property);
                properties.Add(pair.Key, pair.Value);
            }
            return properties;
        }

        private static KeyValuePair<string, object> ReadProperty(JObject property)
        {
            string type = property.Value<string>(PropertyType);
            string name = property.Value<string>(PropertyName);
            object value;
            switch (type)
            {
                case PropertyTypeClear:
                    value = null;
                    break;
                case PropertyTypeBoolean:
                    value = property.Value<bool>(PropertyValue);
                    break;
                case PropertyTypeNumber:
                    switch (property.GetValue(PropertyValue).Type)
                    {
                        case JTokenType.Integer:
                            value = property.Value<int>(PropertyValue);
                            break;
                        case JTokenType.Float:
                            value = property.Value<float>(PropertyValue);
                            break;
                        default:
                            throw new JsonException("Invalid value type");
                    }
                    break;
                case PropertyTypeDatetime:
                    value = property.Value<DateTime>(PropertyValue);
                    break;
                case PropertyTypeString:
                    value = property.Value<string>(PropertyValue);
                    break;
                default:
                    throw new JsonException("Invalid value type");
            }
            return new KeyValuePair<string, object>(name, value);
        }
    }
}
