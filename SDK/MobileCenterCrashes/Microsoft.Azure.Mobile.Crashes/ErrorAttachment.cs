namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Error attachment for error report.
    /// </summary>
    internal class ErrorAttachment
    {
        /// <summary>
        /// Gets or sets a plain text attachment.
        /// </summary>
        /// <value>The text attachment.</value>
        internal string TextAttachment { get; set; }

        /// <summary>
        /// Gets or sets a binary attachment.
        /// </summary>
        /// <value>The binary attachment.</value>
        internal ErrorBinaryAttachment BinaryAttachment { get; set; }

        /// <summary>
        /// Builds an attachment with text and binary suitable for using in <see cref="Crashes.GetErrorAttachment"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if text and data are null.</returns>
        /// <param name="text">Text data.</param>
        /// <param name="data">Binary data.</param>
        /// <param name="filename">Filename to use on reports.</param>
        /// <param name="contentType">Data MIME type.</param>
        internal static ErrorAttachment Attachment(string text, byte[] data, string filename, string contentType)
        {
            return null;
        }

        /// <summary>
        /// Builds an attachment with binary suitable for using in <see cref="Crashes.GetErrorAttachment"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if null data is passed.</returns>
        /// <param name="data">Binary data.</param>
        /// <param name="filename">Filename to use on reports.</param>
        /// <param name="contentType">Data MIME type.</param>
        internal static ErrorAttachment AttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            return null;
        }

        /// <summary>
        /// Builds an attachment with text suitable for using in <see cref="Crashes.GetErrorAttachment"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if null text is passed.</returns>
        /// <param name="text">Text to attach to the error report.</param>
        internal static ErrorAttachment AttachmentWithText(string text)
        {
            return null;
        }
    }
}
