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
    /// Push installation Information.
    /// </summary>
    [JsonObject("push_installation")]
    public partial class PushInstallationLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the PushInstallationLog class.
        /// </summary>
        public PushInstallationLog() { }

        /// <summary>
        /// Initializes a new instance of the PushInstallationLog class.
        /// </summary>
        /// <param name="toffset">Corresponds to the number of milliseconds
        /// elapsed between the time the request is sent and the time the log
        /// is emitted.</param>
        /// <param name="installationId">Globally unique identifier string.
        /// </param>
        /// <param name="pushChannel">The PNS handle for this installation.
        /// </param>
        /// <param name="platform">Device platform.
        /// . Possible values include: 'apns', 'gcm'</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        /// <param name="tags">The list of tags.</param>
        /// <param name="isPatch">If true, tags will be added to existing tags,
        /// else all tags will be overwritten.
        /// </param>
        public PushInstallationLog(long toffset, Device device, string installationId, string pushChannel, string platform, System.Guid? sid = default(System.Guid?), IList<string> tags = default(IList<string>), bool? isPatch = default(bool?))
            : base(toffset, device, sid)
        {
            InstallationId = installationId;
            PushChannel = pushChannel;
            Platform = platform;
            Tags = tags;
            IsPatch = isPatch;
        }

        /// <summary>
        /// Gets or sets globally unique identifier string.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "installation_id")]
        public string InstallationId { get; set; }

        /// <summary>
        /// Gets or sets the PNS handle for this installation.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "push_channel")]
        public string PushChannel { get; set; }

        /// <summary>
        /// Gets or sets device platform.
        /// . Possible values include: 'apns', 'gcm'
        /// </summary>
        [JsonProperty(PropertyName = "platform")]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the list of tags.
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets if true, tags will be added to existing tags, else all
        /// tags will be overwritten.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "is_patch")]
        public bool? IsPatch { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (InstallationId == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "InstallationId");
            }
            if (PushChannel == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "PushChannel");
            }
            if (Platform == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Platform");
            }
            if (Tags != null)
            {
                if (Tags.Count < 0)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MinItems, "Tags", 0);
                }
            }
        }
    }
}

