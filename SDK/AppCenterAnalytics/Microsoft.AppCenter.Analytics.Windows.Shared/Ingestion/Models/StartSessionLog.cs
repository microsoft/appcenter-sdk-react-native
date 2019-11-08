// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Analytics.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    /// <summary>
    /// Required explicit begin session log (a marker event for analytics
    /// service).
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public partial class StartSessionLog : Log
    {
        internal static StartSessionLog Empty = new StartSessionLog();
        internal const string JsonIdentifier = "startSession";

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
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}

