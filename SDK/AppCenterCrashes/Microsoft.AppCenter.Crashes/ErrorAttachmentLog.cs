// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Crashes
{
    public partial class ErrorAttachmentLog
    {
        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            return new ErrorAttachmentLog();
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            return new ErrorAttachmentLog();
        }
    }
}
