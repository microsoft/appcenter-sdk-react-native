using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    internal class PlatformCrashes : PlatformCrashesBase
    {
        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }

        public override SentErrorReportEventHandler SentErrorReport { get; set; }

        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }

        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }

        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

        //public override GetErrorAttachmentCallback GetErrorAttachment { get; set; }

        public override Type BindingType { get; }

        public override bool Enabled { get; set; }

        public override bool HasCrashedInLastSession { get; }

        public override async Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return await Task.FromResult((ErrorReport)null);
        }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
        }

        //public override void TrackException(Exception exception) { }
    }
}