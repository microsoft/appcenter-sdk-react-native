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
    [JsonObject(JsonIdentifier)]
    public partial class NumberProperty : CustomProperty
    {
        internal const string JsonIdentifier = "number";

        /// <summary>
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        public NumberProperty() { }

        /// <summary>
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        /// <param name="value">Number property value.</param>
        public NumberProperty(string name, int value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        /// <param name="value">Number property value.</param>
        public NumberProperty(string name, long value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        /// <param name="value">Number property value.</param>
        public NumberProperty(string name, float value)
            : base(name)
        {
            Value = value;
        }

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
        /// Initializes a new instance of the NumberProperty class.
        /// </summary>
        /// <param name="value">Number property value.</param>
        public NumberProperty(string name, decimal value)
            : base(name)
        {
            Value = value;
        }

        // Use object because number can be either decimal or double (or other numeric types).
        // Decimal has narrower range than double but double has lower precision. Thus, they
        // can't be casted between.

        /// <summary>
        /// Gets or sets number property value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

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

