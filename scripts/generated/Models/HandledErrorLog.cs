// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Handled Error log for managed platforms (such as Xamarin, Unity,
    /// Android Dalvik/ART)
    /// </summary>
    [JsonObject("handledError")]
    public partial class HandledErrorLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the HandledErrorLog class.
        /// </summary>
        public HandledErrorLog() { }

        /// <summary>
        /// Initializes a new instance of the HandledErrorLog class.
        /// </summary>
        /// <param name="exception">Exception associated to the error.</param>
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
        /// <param name="id">Unique identifier for this Error.
        /// </param>
        public HandledErrorLog(Device device, Exception exception, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), System.Guid? id = default(System.Guid?))
            : base(device, timestamp, sid)
        {
            Id = id;
            Exception = exception;
        }

        /// <summary>
        /// Gets or sets unique identifier for this Error.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets exception associated to the error.
        /// </summary>
        [JsonProperty(PropertyName = "exception")]
        public Exception Exception { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Exception == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Exception");
            }
            if (Exception != null)
            {
                Exception.Validate();
            }
        }
    }
}

