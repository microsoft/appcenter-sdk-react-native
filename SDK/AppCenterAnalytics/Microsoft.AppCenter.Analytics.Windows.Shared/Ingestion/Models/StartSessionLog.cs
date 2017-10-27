// Copyright (c) Microsoft Corporation.  All rights reserved.

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
        /// <summary>
        /// Initializes a new instance of the StartSessionLog class.
        /// </summary>
        public StartSessionLog() { }

        internal static StartSessionLog Empty = new StartSessionLog();

        internal const string JsonIdentifier = "start_session";

        /// <summary>
        /// Initializes a new instance of the StartSessionLog class.
        /// </summary>
        /// <param name="timestamp">Log timestamp.</param>
        /// <param name="device">Description of the device emitting the log.</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        public StartSessionLog(DateTime? timestamp, Device device, Guid? sid = default(Guid?))
            : base(timestamp, device, sid)
        {
        }
    }
}

