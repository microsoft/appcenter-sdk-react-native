// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.AppCenter.Ingestion.Models.Serialization;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Clear an existing property.
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public partial class ClearProperty : CustomProperty
    {
        internal const string JsonIdentifier = "clear";

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

        public override object GetValue()
        {
            return null;
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

