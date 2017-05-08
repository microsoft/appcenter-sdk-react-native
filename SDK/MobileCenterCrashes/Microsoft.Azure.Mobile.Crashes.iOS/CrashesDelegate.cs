using Foundation;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class CrashesDelegate : MSCrashesDelegate
    {
        readonly PlatformCrashes _owner;

        internal CrashesDelegate(PlatformCrashes owner)
        {
            _owner = owner;
        }

        public override bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            if (_owner.ShouldProcessErrorReport == null)
            {
                return true;
            }

            var report = ErrorReportCache.GetErrorReport(msReport);
            return _owner.ShouldProcessErrorReport(report);
        }

        public override NSArray AttachmentsWithCrashes(MSCrashes crashes, MSErrorReport msReport)
        {
            if (_owner.GetErrorAttachments == null)
            {
                return null;
            }

            var report = ErrorReportCache.GetErrorReport(msReport);
            var attachments = _owner.GetErrorAttachments(report);
            if (attachments != null)
            {
                var nsArray = new NSMutableArray();
                foreach (var attachment in attachments)
                {
                    nsArray.Add(attachment.internalAttachment);
                }
                return nsArray;
            }

            return null;
        }

        public override void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            if (_owner.SendingErrorReport == null)
            {
                return;
            }

            var report = ErrorReportCache.GetErrorReport(msReport);
            var e = new SendingErrorReportEventArgs();
            e.Report = report;
            _owner.SendingErrorReport(null, e);
        }

        public override void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport msReport)
        {
            if (_owner.SentErrorReport != null)
            {
                var report = ErrorReportCache.GetErrorReport(msReport);
                var e = new SentErrorReportEventArgs();
                e.Report = report;
                _owner.SentErrorReport(null, e);
            }

        }

        public override void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport msReport, NSError error)
        {
            if (_owner.FailedToSendErrorReport != null)
            {
                var report = ErrorReportCache.GetErrorReport(msReport);
                var e = new FailedToSendErrorReportEventArgs();
                e.Report = report;
                e.Exception = error;
                _owner.FailedToSendErrorReport(null, e);
            }
        }
    }
}
