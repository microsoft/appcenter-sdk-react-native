using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{

    public class TestCrashException : Exception { }

    class PlatformCrashes : PlatformCrashesBase
    {
        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }
        public override SentErrorReportEventHandler SentErrorReport { get; set; }
        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        //public override GetErrorAttachmentCallback GetErrorAttachment { get; set; }
        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
        }
        public override bool Enabled { get; set; }
        public override bool HasCrashedInLastSession => false;

        public override Type BindingType => typeof(Crashes);

        public override async Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return null;
        }

      
    }
}