namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class ErrorAttachmentLog
    {
        private static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            return new ErrorAttachmentLog();
        }

        private static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string fileName, string contentType)
        {
            return new ErrorAttachmentLog();
        }
    }
}
