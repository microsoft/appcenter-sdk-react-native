// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Date and time property.
    /// </summary>
    [JsonObject("dateTime")]
    public partial class DateTimeProperty : CustomProperty
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeProperty class.
        /// </summary>
        public DateTimeProperty() { }

        /// <summary>
        /// Initializes a new instance of the DateTimeProperty class.
        /// </summary>
        /// <param name="value">Date time property value.</param>
        public DateTimeProperty(string name, System.DateTime value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets date time property value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public System.DateTime Value { get; set; }

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

