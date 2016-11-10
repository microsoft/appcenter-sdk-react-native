using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public class CrashesDelegate : MSCrashesDelegate
    {
        private readonly PlatformCrashes _owner;

        internal CrashesDelegate(PlatformCrashes owner)
        {
            _owner = owner;
        }

        public override bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            var report = new ErrorReport(msReport);
            return _owner.ShouldProcessErrorReport(report);
        }

        public override MSErrorAttachment AttachmentWithCrashes(MSCrashes crashes, MSErrorReport msReport)
        {
            var report = new ErrorReport(msReport);
            return _owner.GetErrorAttachment(report).ToMSErrorAttachment();
        }

        public override void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            var report = new ErrorReport(msReport);
            var e = new SendingErrorReportEventArgs();
            e.Report = report;
            _owner.SendingErrorReport(null, e);
        }

        public override void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            var report = new ErrorReport(msReport);
            var e = new SentErrorReportEventArgs();
            e.Report = report;
            _owner.SentErrorReport(null, e);
        }

        public override void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport msReport, NSError error)
        {
            var report = new ErrorReport(msReport);
            var e = new FailedToSendErrorReportEventArgs();
            e.Report = report;
            e.Exception = error;
            _owner.FailedToSendErrorReport(null, e);
        }
    }
}
