// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Boolean property.
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public partial class BooleanProperty : CustomProperty
    {
        internal const string JsonIdentifier = "boolean";

        /// <summary>
        /// Initializes a new instance of the BooleanProperty class.
        /// </summary>
        public BooleanProperty() { }

        /// <summary>
        /// Initializes a new instance of the BooleanProperty class.
        /// </summary>
        /// <param name="value">Boolean property value.</param>
        public BooleanProperty(string name, bool value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets boolean property value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public bool Value { get; set; }

        public override object GetValue()
        {
            return Value;
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
        }
    }
}

