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
    /// Describe a MobileCenter.Start API call from the SDK.
    /// </summary>
    [JsonObject("startService")]
    public partial class StartServiceLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the StartServiceLog class.
        /// </summary>
        public StartServiceLog() { }

        /// <summary>
        /// Initializes a new instance of the StartServiceLog class.
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
        /// <param name="services">The list of services of the MobileCenter
        /// Start API call.</param>
        public StartServiceLog(Device device, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), IList<string> services = default(IList<string>))
            : base(device, timestamp, sid)
        {
            Services = services;
        }

        /// <summary>
        /// Gets or sets the list of services of the MobileCenter Start API
        /// call.
        /// </summary>
        [JsonProperty(PropertyName = "services")]
        public IList<string> Services { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Services != null)
            {
                if (Services.Count < 1)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MinItems, "Services", 1);
                }
            }
        }
    }
}

