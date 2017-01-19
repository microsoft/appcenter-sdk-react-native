namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    ///     Binary attachment for error report.
    /// </summary>
    internal class ErrorBinaryAttachment
    {
        /// <summary>
        ///     Gets the binary data file name.
        /// </summary>
        /// <value>The name of the binary data file.</value>
        internal string FileName { get; }

        /// <summary>
        ///     Gets the binary data.
        /// </summary>
        /// <value>The binary data.</value>
        internal byte[] Data { get; }

        /// <summary>
        ///     Gets the content type for the binary data.
        /// </summary>
        /// <value>The MIME type of the binary data.</value>
        internal string ContentType { get; }
    }
}