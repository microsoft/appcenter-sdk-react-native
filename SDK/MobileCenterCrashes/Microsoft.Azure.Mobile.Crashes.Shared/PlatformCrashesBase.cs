using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Object used to share portable code between platforms.
    /// </summary>
    abstract class PlatformCrashesBase : IPlatformCrashes
    {
        public abstract Type BindingType { get; }

        public abstract bool Enabled { get; set; }

        public abstract bool HasCrashedInLastSession { get; }

        public abstract ErrorReport LastSessionCrashReport { get; }

        public void GenerateTestCrash()
        {
            throw new TestCrashException();
        }

        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public abstract SendingErrorReportHandler SendingErrorReport { get; set; }
        public abstract SentErrorReportHandler SentErrorReport { get; set; }
        public abstract FailedToSendErrorHandler FailedToSendErrorReport { get; set; }
        public abstract ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        public abstract GetErrorAttachmentCallback GetErrorAttachment { get; set; }
        //public abstract void TrackException(Exception exception);
    }
}
