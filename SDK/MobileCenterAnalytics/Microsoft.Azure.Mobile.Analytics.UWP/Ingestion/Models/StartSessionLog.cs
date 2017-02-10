// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Mobile.Ingestion.Models;
    using Newtonsoft.Json;
    using System.Linq;
    using System;

    /// <summary>
    /// Required explicit begin session log (a marker event for analytics
    /// service).
    /// </summary>
    [JsonObject("start_session")]
    public partial class StartSessionLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the StartSessionLog class.
        /// </summary>
        public StartSessionLog() { }

        internal static StartSessionLog Empty = new StartSessionLog();

        public override string GetJsonIdentifier()
        {
            return "start_session";
        }

        /// <summary>
        /// Initializes a new instance of the StartSessionLog class.
        /// </summary>
        /// <param name="toffset">Corresponds to the number of milliseconds
        /// elapsed between the time the request is sent and the time the log
        /// is emitted.</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        public StartSessionLog(long toffset, Mobile.Ingestion.Models.Device device, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, sid)
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

