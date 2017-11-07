// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Number property.
    /// </summary>
    [JsonObject("number")]
    public partial class NumberProperty : CustomProperty
    {
        /// <summary>
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        public NumberProperty() { }

        /// <summary>
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        /// <param name="value">Number property value.</param>
        public NumberProperty(string name, double value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets number property value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public double Value { get; set; }

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

