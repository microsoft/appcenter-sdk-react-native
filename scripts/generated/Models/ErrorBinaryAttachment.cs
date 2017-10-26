// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.UWP;
    using Microsoft.AppCenter.UWP.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Binary attachment for error log.
    /// </summary>
    public partial class ErrorBinaryAttachment
    {
        /// <summary>
        /// Initializes a new instance of the ErrorBinaryAttachment class.
        /// </summary>
        public ErrorBinaryAttachment() { }

        /// <summary>
        /// Initializes a new instance of the ErrorBinaryAttachment class.
        /// </summary>
        /// <param name="contentType">Content type for binary data.</param>
        /// <param name="data">Binary data.</param>
        /// <param name="fileName">File name for binary data.</param>
        public ErrorBinaryAttachment(string contentType, byte[] data, string fileName = default(string))
        {
            ContentType = contentType;
            FileName = fileName;
            Data = data;
        }

        /// <summary>
        /// Gets or sets content type for binary data.
        /// </summary>
        [JsonProperty(PropertyName = "content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets file name for binary data.
        /// </summary>
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets binary data.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public byte[] Data { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
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

