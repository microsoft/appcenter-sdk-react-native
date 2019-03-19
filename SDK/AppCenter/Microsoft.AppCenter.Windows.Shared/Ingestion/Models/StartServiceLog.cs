// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models
{
    /// <summary>
    /// Log type for sending information about which services have been started
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public class StartServiceLog : Log
    {
        internal const string JsonIdentifier = "startService";

        /// <summary>
        /// Initializes a new instance of the StartServiceLog class.
        /// </summary>
        public StartServiceLog()
        {
            Services = new List<string>();
        }

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
        public StartServiceLog(Device device, DateTime? timestamp = default(DateTime?), Guid? sid = default(Guid?), IList<string> services = default(IList<string>))
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
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Services != null)
            {
                if (Services.Count < 1)
                {
                    throw new ValidationException(ValidationException.Rule.MinItems, nameof(Services), 1);
                }
            }
        }
    }
}
