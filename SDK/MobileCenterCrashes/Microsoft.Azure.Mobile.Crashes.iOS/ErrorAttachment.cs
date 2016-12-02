using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    /// <summary>
    /// Error attachment for error report.
    /// </summary>
    public class ErrorAttachment
    {
        internal MSErrorAttachment internalAttachment { get; }
        private ErrorBinaryAttachment internalBinaryAttachment;

        private ErrorAttachment(MSErrorAttachment iosAttachment)
        {
            internalAttachment = iosAttachment;
        }

        /// <summary>
        /// Gets or sets a plain text attachment.
        /// </summary>
        /// <value>The text attachment.</value>
        public string TextAttachment => internalAttachment.TextAttachment;

        /// <summary>
        /// Gets or sets a binary attachment.
        /// </summary>
        /// <value>The binary attachment.</value>
        public ErrorBinaryAttachment BinaryAttachment
        { 
            get
            {
                if (internalBinaryAttachment == null)
                {
                    internalBinaryAttachment = new ErrorBinaryAttachment(internalAttachment.BinaryAttachment);
                }
                return internalBinaryAttachment; 
            }
            set
            {
                internalAttachment.BinaryAttachment = value.internalBinaryAttachment;
                internalBinaryAttachment = null;
            }
        }

        /// <summary>
        /// Builds an attachment with text and binary suitable for using in <see cref="Crashes.GetErrorAttachment"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if text and data are null.</returns>
        /// <param name="text">Text data.</param>
        /// <param name="data">Binary data.</param>
        /// <param name="filename">Filename to use on reports.</param>
        /// <param name="contentType">Data MIME type.</param>
        public static ErrorAttachment Attachment(string text, byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSErrorAttachment iosAttachment = MSErrorAttachment.AttachmentWithText(text, nsdata, filename, contentType);
            return new ErrorAttachment(iosAttachment);
        }

        /// <summary>
        /// Builds an attachment with binary suitable for using in <see cref="Crashes.GetErrorAttachment"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if null data is passed.</returns>
        /// <param name="data">Binary data.</param>
        /// <param name="filename">Filename to use on reports.</param>
        /// <param name="contentType">Data MIME type.</param>
        public static ErrorAttachment AttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSErrorAttachment iosAttachment = MSErrorAttachment.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachment(iosAttachment);
        }

        /// <summary>
        /// Builds an attachment with text suitable for using in <see cref="Crashes.GetErrorAttachment"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if null text is passed.</returns>
        /// <param name="text">Text to attach to the error report.</param>
        public static ErrorAttachment AttachmentWithText(string text)
        {
            MSErrorAttachment iosAttachment = MSErrorAttachment.AttachmentWithText(text);
            return new ErrorAttachment(iosAttachment);
        }

    }
}
