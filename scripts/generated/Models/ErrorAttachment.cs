// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Attachment for error log.
    /// </summary>
    public partial class ErrorAttachment
    {
        /// <summary>
        /// Initializes a new instance of the ErrorAttachment class.
        /// </summary>
        public ErrorAttachment() { }

        /// <summary>
        /// Initializes a new instance of the ErrorAttachment class.
        /// </summary>
        /// <param name="textAttachment">Plain text attachment.</param>
        /// <param name="binaryAttachment">Binary attachment.</param>
        public ErrorAttachment(string textAttachment = default(string), ErrorBinaryAttachment binaryAttachment = default(ErrorBinaryAttachment))
        {
            TextAttachment = textAttachment;
            BinaryAttachment = binaryAttachment;
        }

        /// <summary>
        /// Gets or sets plain text attachment.
        /// </summary>
        [JsonProperty(PropertyName = "text_attachment")]
        public string TextAttachment { get; set; }

        /// <summary>
        /// Gets or sets binary attachment.
        /// </summary>
        [JsonProperty(PropertyName = "binary_attachment")]
        public ErrorBinaryAttachment BinaryAttachment { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (BinaryAttachment != null)
            {
                BinaryAttachment.Validate();
            }
        }
    }
}

