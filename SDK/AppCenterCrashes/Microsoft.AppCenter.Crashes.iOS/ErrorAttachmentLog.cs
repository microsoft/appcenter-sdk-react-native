// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using Microsoft.AppCenter.Crashes.iOS.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class ErrorAttachmentLog
    {
        internal MSErrorAttachmentLog internalAttachment { get; }

        ErrorAttachmentLog(MSErrorAttachmentLog iosAttachment)
        {
            internalAttachment = iosAttachment;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            MSErrorAttachmentLog iosAttachment = MSErrorAttachmentLog.AttachmentWithText(text, fileName);
            return new ErrorAttachmentLog(iosAttachment);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSErrorAttachmentLog iosAttachment = MSErrorAttachmentLog.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachmentLog(iosAttachment);
        }
    }
}
