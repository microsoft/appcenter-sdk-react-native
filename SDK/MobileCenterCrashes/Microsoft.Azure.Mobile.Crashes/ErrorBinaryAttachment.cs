namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    ///     Binary attachment for error report.
    /// </summary>
    public class ErrorBinaryAttachment
    {
        /// <summary>
        ///     Gets the binary data file name.
        /// </summary>
        /// <value>The name of the binary data file.</value>
        public string FileName { get; }

        /// <summary>
        ///     Gets the binary data.
        /// </summary>
        /// <value>The binary data.</value>
        public byte[] Data { get; }

        /// <summary>
        ///     Gets the content type for the binary data.
        /// </summary>
        /// <value>The MIME type of the binary data.</value>
        public string ContentType { get; }
    }
}