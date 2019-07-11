// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Newtonsoft.Json;

    public partial class Log
    {
        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public Log()
        {
            CustomInit();
        }

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
        /// <param name="userId">optional string used for associating logs with
        /// users.
        /// </param>
        public Log(Device device, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), string userId = default(string))
        {
            Timestamp = timestamp;
            Sid = sid;
            UserId = userId;
            Device = device;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults.
        /// </summary>
        partial void CustomInit();

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
        /// Gets or sets optional string used for associating logs with users.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

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
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Device");
            }
            if (Device != null)
            {
                Device.Validate();
            }
        }
    }
}
