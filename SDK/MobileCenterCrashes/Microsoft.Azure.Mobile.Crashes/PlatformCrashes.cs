using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    class PlatformCrashes : PlatformCrashesBase
    {
        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public override SendingErrorReportHandler SendingErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override SentErrorReportHandler SentErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override FailedToSendErrorHandler FailedToSendErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override GetErrorAttachmentCallback GetErrorAttachment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
            throw new NotImplementedException();
        }

        public override Type BindingType
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool HasCrashedInLastSession
        {
            get { throw new NotImplementedException(); }
        }

        public override ErrorReport LastSessionCrashReport
        {
            get { throw new NotImplementedException(); }
        }

        //public override void TrackException(Exception exception)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
