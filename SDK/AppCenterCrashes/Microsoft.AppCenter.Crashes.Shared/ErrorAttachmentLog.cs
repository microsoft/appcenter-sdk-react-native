// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Error attachment for error report.
    /// </summary>
    public partial class ErrorAttachmentLog
    {
        /// <summary>
        /// Builds an attachment with text suitable for using in <see cref="Crashes.GetErrorAttachments"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if null text is passed.</returns>
        /// <param name="text">Text to attach to the error report.</param>
        /// <param name="fileName">File name to use on reports.</param>
        public static ErrorAttachmentLog AttachmentWithText(string text, string fileName)
        {
            return PlatformAttachmentWithText(text, fileName);
        }

        /// <summary>
        /// Builds an attachment with binary suitable for using in <see cref="Crashes.GetErrorAttachments"/>.
        /// </summary>
        /// <returns>Error attachment or <c>null</c> if null data is passed.</returns>
        /// <param name="data">Binary data.</param>
        /// <param name="fileName">File name to use on reports.</param>
        /// <param name="contentType">Data MIME type.</param>
        public static ErrorAttachmentLog AttachmentWithBinary(byte[] data, string fileName, string contentType)
        {
            return PlatformAttachmentWithBinary(data, fileName, contentType);
        }
    }
}
