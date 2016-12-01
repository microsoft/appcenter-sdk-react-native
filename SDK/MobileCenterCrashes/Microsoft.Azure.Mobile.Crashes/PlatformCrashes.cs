using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    class PlatformCrashes : PlatformCrashesBase
    {
        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public override SendingErrorReportEventHandler SendingErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override SentErrorReportEventHandler SentErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override ErrorAttachmentForErrorReportCallback ErrorAttachmentForErrorReport
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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
