// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.Ingestion;
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
        public Log(long toffset, Device device, System.Guid? sid = default(System.Guid?))
        {
            Toffset = toffset;
            Sid = sid;
            Device = device;
        }

        /// <summary>
        /// Gets or sets corresponds to the number of milliseconds elapsed
        /// between the time the request is sent and the time the log is
        /// emitted.
        /// </summary>
        [JsonProperty(PropertyName = "toffset")]
        public long Toffset { get; set; }

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

