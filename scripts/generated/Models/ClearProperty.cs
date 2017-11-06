// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Clear an existing property.
    /// </summary>
    [JsonObject("clear")]
    public partial class ClearProperty : CustomProperty
    {
        /// <summary>
        /// Initializes a new instance of the ClearProperty class.
        /// </summary>
        public ClearProperty() { }

        /// <summary>
        /// Initializes a new instance of the ClearProperty class.
        /// </summary>
        public ClearProperty(string name)
            : base(name)
        {
        }

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

