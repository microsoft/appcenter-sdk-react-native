// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Push.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.AppCenter.Ingestion.Models;
    using Newtonsoft.Json;
    using System.Linq;

    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [JsonObject(JsonIdentifier)]
    public class PushInstallationLog : Log
    {
        internal const string JsonIdentifier = "pushInstallation";

        /// <summary>
        /// Initializes a new instance of the PushInstallationLog class.
        /// </summary>
        public PushInstallationLog() { }

        /// <summary>
        /// Initializes a new instance of the PushInstallationLog class.
        /// </summary>
        /// <param name="pushToken">The PNS handle for this installation.
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
        public PushInstallationLog(System.DateTime? timestamp, Device device, string pushToken, System.Guid? sid = default(System.Guid?))
            : base(device, timestamp, sid)
        {
            PushToken = pushToken;
        }

        /// <summary>
        /// Gets or sets the PNS handle for this installation.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "pushToken")]
        public string PushToken { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (PushToken == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(PushToken));
            }
        }
    }
}
