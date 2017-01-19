namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidICrashListener = Com.Microsoft.Azure.Mobile.Crashes.ICrashesListener;
    using AndroidErrorAttachment = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidErrorAttachment;
    using AndroidErrorReport = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidErrorReport;

    class AndroidCrashListener : Java.Lang.Object, AndroidICrashListener
    {
        private readonly PlatformCrashes _owner;

        public AndroidCrashListener(PlatformCrashes owner)
        {
            _owner = owner;
        }

        public AndroidErrorAttachment GetErrorAttachment(AndroidErrorReport androidReport)
        {
            //if (_owner.GetErrorAttachment == null)
            //{
            //    return null;
            //}

            //var report = ErrorReportCache.GetErrorReport(androidReport);
            //var attachment = _owner.GetErrorAttachment(report);
            //if (attachment != null)
            //{
            //    return attachment.internalAttachment;
            //}

            return null;
        }

        public void OnBeforeSending(AndroidErrorReport androidReport)
        {
            if (_owner.SendingErrorReport == null)
            {
                return;
            }
            var report = ErrorReportCache.GetErrorReport(androidReport);
            var e = new  SendingErrorReportEventArgs();
            e.Report = report;
            _owner.SendingErrorReport(null, e);
        }

        public void OnSendingFailed(AndroidErrorReport androidReport, Java.Lang.Exception exception)
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
