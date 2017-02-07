// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Event log.
    /// </summary>
    [JsonObject("event")]
    public partial class EventLog : LogWithProperties
    {
        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        public EventLog() { }

        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        /// <param name="toffset">Corresponds to the number of milliseconds
        /// elapsed between the time the request is sent and the time the log
        /// is emitted.</param>
        /// <param name="id">Unique identifier for this event.
        /// </param>
        /// <param name="name">Name of the event.
        /// </param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        /// <param name="properties">Additional key/value pair parameters.
        /// </param>
        public EventLog(long toffset, Device device, System.Guid id, string name, System.Guid? sid = default(System.Guid?), IDictionary<string, string> properties = default(IDictionary<string, string>))
            : base(toffset, device, sid, properties)
        {
            Id = id;
            Name = name;
        }

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
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Name == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Name");
            }
        }
    }
}

