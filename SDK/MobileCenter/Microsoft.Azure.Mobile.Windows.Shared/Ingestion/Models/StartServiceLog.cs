using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    using Newtonsoft.Json;

    public class StartServiceLog : Log
    {
        internal const string JsonIdentifier = "StartServiceLog";

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary
        public StartServiceLog() { }

        /// <summary>
        /// Initializes a new instance of the Log class
        /// </summary>
        /// <param name="toffset">>Corresponds to the number of milliseconds
        /// elapsed between the time the request is sent and the time the log
        /// is emitted.</param>
        /// <param name="device">Device and SDK information</param>
        /// <param name="services">Names of services which started with SDK</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.</param>
        public StartServiceLog(long toffset, Device device, IEnumerable<string> services, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, sid)
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
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (Services == null)
            {
                throw new Rest.ValidationException(Rest.ValidationRules.CannotBeNull, nameof(Services));
            }
        }
    }
}
