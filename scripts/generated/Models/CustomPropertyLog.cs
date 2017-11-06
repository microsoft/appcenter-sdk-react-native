// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Set or remove custom properties.
    /// </summary>
    [JsonObject("customProperties")]
    public partial class CustomPropertyLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the CustomPropertyLog class.
        /// </summary>
        public CustomPropertyLog() { }

        /// <summary>
        /// Initializes a new instance of the CustomPropertyLog class.
        /// </summary>
        /// <param name="timestamp">Log timestamp, example:
        /// '2017-03-13T18:05:42Z'.
        /// </param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        /// <param name="properties">Custom property changes.</param>
        public CustomPropertyLog(Device device, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), IList<CustomProperty> properties = default(IList<CustomProperty>))
            : base(device, timestamp, sid)
        {
            Properties = properties;
        }

        /// <summary>
        /// Gets or sets custom property changes.
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public IList<CustomProperty> Properties { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Properties != null)
            {
                if (Properties.Count > 60)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MaxItems, "Properties", 60);
                }
                if (Properties.Count < 1)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MinItems, "Properties", 1);
                }
                foreach (var element in Properties)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}

