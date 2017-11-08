// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Log
    {
        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public Log() { }

        /// <summary>
        /// Initializes a new instance of the Log class.
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
        public Log(Device device, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?))
        {
            Timestamp = timestamp;
            Sid = sid;
            Device = device;
        }

        /// <summary>
        /// Gets or sets log timestamp, example: '2017-03-13T18:05:42Z'.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public System.DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets when tracking an analytics session, logs can be part
        /// of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "sid")]
        public System.Guid? Sid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "device")]
        public Device Device { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Device == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Device");
            }
            if (Device != null)
            {
                Device.Validate();
            }
        }
    }
}

