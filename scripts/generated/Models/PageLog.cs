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
    /// Page view log (as in screens or activities).
    /// </summary>
    [JsonObject("page")]
    public partial class PageLog : LogWithProperties
    {
        /// <summary>
        /// Initializes a new instance of the PageLog class.
        /// </summary>
        public PageLog() { }

        /// <summary>
        /// Initializes a new instance of the PageLog class.
        /// </summary>
        /// <param name="name">Name of the page.
        /// </param>
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
        /// <param name="properties">Additional key/value pair parameters.
        /// </param>
        public PageLog(Device device, string name, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), IDictionary<string, string> properties = default(IDictionary<string, string>))
            : base(device, timestamp, sid, properties)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets name of the page.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Name == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Name");
            }
        }
    }
}

