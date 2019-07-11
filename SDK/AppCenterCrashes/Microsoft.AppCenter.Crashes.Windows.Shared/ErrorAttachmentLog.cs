// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Error attachment log.
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public partial class ErrorAttachmentLog : Log
    {
        internal const string JsonIdentifier = "errorAttachment";

        private const string ContentTypePlainText = "text/plain";

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            if (text == null)
            {
                return null;
            }
            var data = Encoding.UTF8.GetBytes(text);
            return PlatformAttachmentWithBinary(data, fileName, ContentTypePlainText);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string fileName, string contentType)
        {
            if (data == null)
            {
                return null;
            }
            return new ErrorAttachmentLog()
            {
                Data = data,
                FileName = fileName,
                ContentType = contentType
            };
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
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (ContentType == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "ContentType");
            }
            if (Data == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Data");
            }
        }

        /// <summary>
        /// Check all required fields are present.
        /// </summary>
        /// <returns>True if all required fields are present.</returns>
        public bool ValidatePropertiesForAttachment()
        {
            var isErrorIdValid = ErrorId != System.Guid.Empty;
            var isAttachmentIdValid = Id != System.Guid.Empty;
            var isAttachmentDataValid = Data != null && Data.Length > 0;
            var isContentTypeValid = !string.IsNullOrEmpty(ContentType);
            return isErrorIdValid && isAttachmentIdValid && isAttachmentDataValid && isContentTypeValid;
        }
    }
}
