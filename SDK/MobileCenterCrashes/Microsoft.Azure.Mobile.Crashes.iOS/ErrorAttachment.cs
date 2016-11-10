using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public class ErrorAttachment
    {
        internal MSErrorAttachment internalAttachment { get; }

        internal ErrorAttachment(MSErrorAttachment iosAttachment)
        {
            internalAttachment = iosAttachment;
        }

        public ErrorAttachment(string text)
        {
            internalAttachment = MSErrorAttachment.AttachmentWithText(text);
        }

        public ErrorAttachment(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            internalAttachment = MSErrorAttachment.AttachmentWithBinaryData(nsdata, filename, contentType);
        }

        public ErrorAttachment(string text, byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            internalAttachment = MSErrorAttachment.AttachmentWithText(text, nsdata, filename, contentType);
        }

        public string TextAttachment => internalAttachment.TextAttachment;

        public ErrorBinaryAttachment BinaryAttachment
        { 
            get
            {
                return new ErrorBinaryAttachment(internalAttachment.BinaryAttachment);
            }
            set
            {
                internalAttachment.BinaryAttachment = value.internalBinaryAttachment;
            }
        }
    }
}
