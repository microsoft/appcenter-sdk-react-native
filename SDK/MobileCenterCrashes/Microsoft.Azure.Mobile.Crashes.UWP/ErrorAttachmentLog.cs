namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class ErrorAttachmentLog
    {
        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            return new ErrorAttachmentLog();
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string fileName, string contentType)
        {
            return new ErrorAttachmentLog();
        }
    }
}
