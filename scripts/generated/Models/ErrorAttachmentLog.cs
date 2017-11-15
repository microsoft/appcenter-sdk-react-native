// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Error attachment log.
    /// </summary>
    [JsonObject("errorAttachment")]
    public partial class ErrorAttachmentLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the ErrorAttachmentLog class.
        /// </summary>
        public ErrorAttachmentLog() { }

        /// <summary>
        /// Initializes a new instance of the ErrorAttachmentLog class.
        /// </summary>
        /// <param name="id">Error attachment identifier.</param>
        /// <param name="errorId">Error log identifier to attach this log
        /// to.</param>
        /// <param name="contentType">Content type (text/plain for
        /// text).</param>
        /// <param name="data">Data encoded as base 64.</param>
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
        /// <param name="fileName">File name.</param>
        public ErrorAttachmentLog(Device device, System.Guid id, System.Guid errorId, string contentType, byte[] data, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), string fileName = default(string))
            : base(device, timestamp, sid)
        {
            Id = id;
            ErrorId = errorId;
            ContentType = contentType;
            FileName = fileName;
            Data = data;
        }

        /// <summary>
        /// Gets or sets error attachment identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// Gets or sets error log identifier to attach this log to.
        /// </summary>
        [JsonProperty(PropertyName = "errorId")]
        public System.Guid ErrorId { get; set; }

        /// <summary>
        /// Gets or sets content type (text/plain for text).
        /// </summary>
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets data encoded as base 64.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public byte[] Data { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (ContentType == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "ContentType");
            }
            if (Data == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Data");
            }
        }
    }
}

