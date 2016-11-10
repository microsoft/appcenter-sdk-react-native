using System;
namespace Microsoft.Azure.Mobile.Crashes
{
    public delegate bool ShouldProcessErrorReportCallback(ErrorReport report);
    public delegate ErrorAttachment GetErrorAttachmentCallback(ErrorReport report);
    public delegate void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e);
    public delegate void SentErrorReportHandler(object sender, SentErrorReportEventArgs e);
    public delegate void FailedToSendErrorHandler(object sender, FailedToSendErrorReportEventArgs e);
}
