// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    [JsonObject("boolean")]
    public partial class BooleanProperty : CustomProperty
    {
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

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}

