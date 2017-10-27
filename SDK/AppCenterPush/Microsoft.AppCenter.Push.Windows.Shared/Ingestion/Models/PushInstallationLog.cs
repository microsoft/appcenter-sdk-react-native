using System;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Push.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [JsonObject(JsonIdentifier)]
    public class PushInstallationLog : Log
    {
        internal const string JsonIdentifier = "push_installation";

        /// <summary>
        /// Initializes a new instance of the PushInstallationLog class.
        /// </summary>
        /// <param name="timestamp">Log timestamp.</param>
        /// <param name="device">Description of the device emitting the log.</param>
        /// <param name="pushToken">The Windows Push Notification handle for this installation.</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        public PushInstallationLog(DateTime? timestamp, Device device, string pushToken, Guid? sid = default(Guid?))
            : base(timestamp, device, sid)
        {
            PushToken = pushToken;
        }

        /// <summary>
        /// The Windows Push Notification handle for this installation.
        /// </summary>
        [JsonProperty(PropertyName = "push_token")]
        public string PushToken { get; set; }

        /// <summary>
        /// Validate the PushInstallationLog
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if PushToken is null or empty
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(this.PushToken))
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "PushToken");
            }
        }
    }
}
