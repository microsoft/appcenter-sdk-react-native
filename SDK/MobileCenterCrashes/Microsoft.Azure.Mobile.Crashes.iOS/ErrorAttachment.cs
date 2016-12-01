using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public class ErrorAttachment
    {
        internal MSErrorAttachment internalAttachment { get; }

        private ErrorAttachment(MSErrorAttachment iosAttachment)
        {
            internalAttachment = iosAttachment;
        }

        public static ErrorAttachment Attachment(string text, byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSErrorAttachment iosAttachment = MSErrorAttachment.AttachmentWithText(text, nsdata, filename, contentType);
            return new ErrorAttachment(iosAttachment);
        }

        public static ErrorAttachment AttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSErrorAttachment iosAttachment = MSErrorAttachment.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachment(iosAttachment);
        }

        public static ErrorAttachment AttachmentWithText(string text)
        {
            MSErrorAttachment iosAttachment = MSErrorAttachment.AttachmentWithText(text);
            return new ErrorAttachment(iosAttachment);
        }

        public string TextAttachment => internalAttachment.TextAttachment;

        private ErrorBinaryAttachment internalBinaryAttachment;

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
    }
}
