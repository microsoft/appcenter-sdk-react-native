// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Required explicit begin session log (a marker event for analytics
    /// service).
    /// </summary>
    [JsonObject("startSession")]
    public partial class StartSessionLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the StartSessionLog class.
        /// </summary>
        public StartSessionLog() { }

        /// <summary>
        /// Initializes a new instance of the StartSessionLog class.
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
        public StartSessionLog(Device device, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?))
            : base(device, timestamp, sid)
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

