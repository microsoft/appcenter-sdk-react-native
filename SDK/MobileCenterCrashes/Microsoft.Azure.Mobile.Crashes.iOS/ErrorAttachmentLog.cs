using Foundation;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
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
