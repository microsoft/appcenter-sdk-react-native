using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    internal class PlatformCrashes : PlatformCrashesBase
    {
        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }

        public override SentErrorReportEventHandler SentErrorReport { get; set; }

        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }

        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }

        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

        public override Type BindingType { get; }

        public override bool Enabled { get; set; }

        public override bool HasCrashedInLastSession { get; }

        public override Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return Task.FromResult((ErrorReport)null);
        }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
        }
    }
}
