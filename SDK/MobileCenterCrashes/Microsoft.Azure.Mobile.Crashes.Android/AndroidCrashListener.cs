using Com.Microsoft.Azure.Mobile.Crashes;
using Com.Microsoft.Azure.Mobile.Crashes.Model;
using Java.Lang;
using Java.Util;

namespace Microsoft.Azure.Mobile.Crashes
{
    class AndroidCrashListener : Object, ICrashesListener
    {
        readonly PlatformCrashes _owner;

        public AndroidCrashListener(PlatformCrashes owner)
        {
            _owner = owner;
        }

        public IIterable GetErrorAttachments(AndroidErrorReport androidReport)
        {
            if (_owner.GetErrorAttachments == null)
            {
                return null;
            }

            var report = ErrorReportCache.GetErrorReport(androidReport);
            var attachments = _owner.GetErrorAttachments(report);
            if (attachments != null)
            {
                var attachmentList = new ArrayList();
                foreach (var attachment in attachments)
                {
                    attachmentList.Add(attachment.internalAttachment);
                }
                return attachmentList;
            }

            return null;
        }

        public void OnBeforeSending(AndroidErrorReport androidReport)
        {
            if (_owner.SendingErrorReport == null)
            {
                return;
            }
            var report = ErrorReportCache.GetErrorReport(androidReport);
            var e = new SendingErrorReportEventArgs();
            e.Report = report;
            _owner.SendingErrorReport(null, e);
        }

        public void OnSendingFailed(AndroidErrorReport androidReport, Exception exception)
        {
            if (_owner.FailedToSendErrorReport == null)
            {
                return;
            }
            var report = ErrorReportCache.GetErrorReport(androidReport);
            var e = new FailedToSendErrorReportEventArgs();
            e.Report = report;
            e.Exception = exception;
            _owner.FailedToSendErrorReport(null, e);
        }

        public void OnSendingSucceeded(AndroidErrorReport androidReport)
        {
            if (_owner.SentErrorReport == null)
            {
                return;
            }
            var report = ErrorReportCache.GetErrorReport(androidReport);
            var e = new SentErrorReportEventArgs();
            e.Report = report;
            _owner.SentErrorReport(null, e);
        }

        public bool ShouldAwaitUserConfirmation()
        {
            if (_owner.ShouldAwaitUserConfirmation != null)
            {
                return _owner.ShouldAwaitUserConfirmation();
            }

            return false;
        }

        public bool ShouldProcess(AndroidErrorReport androidReport)
        {
            if (_owner.ShouldProcessErrorReport == null)
            {
                return true;
            }
            var report = ErrorReportCache.GetErrorReport(androidReport);
            return _owner.ShouldProcessErrorReport(report);
        }
    }
}
