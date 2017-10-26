using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models
{
    /// <summary>
    /// Log type for sending information about which services have been started
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public class StartServiceLog : Log
    {
        internal const string JsonIdentifier = "start_service";

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public StartServiceLog()
        {
            Services = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the Log class
        /// </summary>
        /// <param name="timestamp">Log timestamp.</param>
        /// <param name="device">Description of the device emitting the log.</param>
        /// <param name="services">Names of services which started with SDK</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.</param>
        public StartServiceLog(DateTime? timestamp, Device device, IEnumerable<string> services, Guid? sid = default(Guid?))
            : base(timestamp, device, sid)
        {
            Services = new List<string>(services);
        }

        /// <summary>
        /// Services names which have been started
        /// </summary>
        [JsonProperty(PropertyName = "services")]
        public List<string> Services { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (Services == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(Services));
            }
        }
    }
}
