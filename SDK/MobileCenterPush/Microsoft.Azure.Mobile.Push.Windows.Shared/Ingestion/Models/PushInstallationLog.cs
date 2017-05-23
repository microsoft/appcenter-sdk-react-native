using Microsoft.Azure.Mobile.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Push.Ingestion.Models
{
    [JsonObject(JsonIdentifier)]
    public class PushInstallationLog : Log
    {
        internal const string JsonIdentifier = "push_installation";

        public PushInstallationLog(long toffset, Mobile.Ingestion.Models.Device device, string pushToken, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, sid)
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
