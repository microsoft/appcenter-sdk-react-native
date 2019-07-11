// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Analytics.Ingestion.Models
{
    using Microsoft.AppCenter.Ingestion.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Event log.
    /// </summary>
    [Newtonsoft.Json.JsonObject(JsonIdentifier)]
    public partial class EventLog : LogWithProperties
    {
        internal const string JsonIdentifier = "event";

        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        public EventLog()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        /// <param name="id">Unique identifier for this event.
        /// </param>
        /// <param name="name">Name of the event.
        /// </param>
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
        /// <param name="properties">Additional key/value pair parameters.
        /// </param>
        public EventLog(Device device, System.Guid id, string name, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), string userId = default(string), IDictionary<string, string> properties = default(IDictionary<string, string>))
            : base(device, timestamp, sid, userId, properties)
        {
            Id = id;
            Name = name;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets unique identifier for this event.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of the event.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Name == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
            }
            if (Name != null)
            {
                if (Name.Length > 256)
                {
                    throw new ValidationException(ValidationException.Rule.MaxLength, "Name", 256);
                }
            }
        }
    }
}
