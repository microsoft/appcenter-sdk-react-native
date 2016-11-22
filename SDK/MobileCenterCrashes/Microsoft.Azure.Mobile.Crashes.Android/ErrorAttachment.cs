using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidUtils = Com.Microsoft.Azure.Mobile.Crashes.ErrorAttachments;
    using AndroidErrorAttachment = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidErrorAttachment;

    public class ErrorAttachment
    {
        internal AndroidErrorAttachment internalAttachment { get; }

        internal ErrorAttachment(AndroidErrorAttachment androidAttachment)
        {
            internalAttachment = androidAttachment;
        }

          // Public Constructors
        public ErrorAttachment(string text)
        {
            internalAttachment = AndroidUtils.AttachmentWithText(text);
        }

        public ErrorAttachment(byte[] data, string filename, string contentType)
        {
            internalAttachment = AndroidUtils.AttachmentWithBinary(data, filename, contentType);
        }

        public ErrorAttachment(string text, byte[] data, string filename, string contentType)
        {
            internalAttachment = AndroidUtils.Attachment(text, data, filename, contentType);
        }

        // Properties
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
