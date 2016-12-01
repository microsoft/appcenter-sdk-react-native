using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidErrorAttachments = Com.Microsoft.Azure.Mobile.Crashes.ErrorAttachments;
    using AndroidErrorAttachment = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidErrorAttachment;

    public class ErrorAttachment
    {
        internal AndroidErrorAttachment internalAttachment { get; }

        private ErrorAttachment(AndroidErrorAttachment androidAttachment)
        {
            internalAttachment = androidAttachment;
        }

        public static ErrorAttachment Attachment(string text, byte[] data, string filename, string contentType)
        {
            AndroidErrorAttachment androidAttachment = AndroidErrorAttachments.Attachment(text, data, filename, contentType);
            return new ErrorAttachment(androidAttachment);
        }

        public static ErrorAttachment AttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            AndroidErrorAttachment androidAttachment = AndroidErrorAttachments.AttachmentWithBinary(data, filename, contentType);
            return new ErrorAttachment(androidAttachment);
        }

        public static ErrorAttachment AttachmentWithText(string text)
        {
            AndroidErrorAttachment androidAttachment = AndroidErrorAttachments.AttachmentWithText(text);
            return new ErrorAttachment(androidAttachment);
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
