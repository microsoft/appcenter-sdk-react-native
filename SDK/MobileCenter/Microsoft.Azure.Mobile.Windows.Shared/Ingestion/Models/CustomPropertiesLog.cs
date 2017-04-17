using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    [JsonObject(JsonIdentifier)]
    public class CustomPropertiesLog : Log
    {
        internal const string JsonIdentifier = "custom_properties";

        /// <summary>
        /// Key/value pair properties.
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (Properties == null)
            {
                throw new Rest.ValidationException(Rest.ValidationRules.CannotBeNull, nameof(Properties));
            }
        }
    }
}
