// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public abstract class Log
    {
        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        protected Log() { }

        /// <summary>
        /// Initializes a new instance of the Log class.
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
        protected Log(DateTime? timestamp, Device device, Guid? sid = default(Guid?))
        {
            Timestamp = timestamp;
            Sid = sid;
            Device = device;
        }

        /// <summary>
        /// Log timestamp.
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? Timestamp { get; set; }

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
        public Guid? Sid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "device")]
        public Device Device { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Device == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(Device));
            }
            Device.Validate();          
        }
    }
}

