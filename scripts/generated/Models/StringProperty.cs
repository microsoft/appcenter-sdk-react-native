// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// String property.
    /// </summary>
    [JsonObject("string")]
    public partial class StringProperty : CustomProperty
    {
        /// <summary>
        /// Initializes a new instance of the StringProperty class.
        /// </summary>
        public StringProperty() { }

        /// <summary>
        /// Initializes a new instance of the StringProperty class.
        /// </summary>
        /// <param name="value">String property value.</param>
        public StringProperty(string name, string value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets string property value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Value == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Value");
            }
            if (Value != null)
            {
                if (Value.Length > 128)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MaxLength, "Value", 128);
                }
            }
        }
    }
}

