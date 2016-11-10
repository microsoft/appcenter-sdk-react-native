namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidErrorAttachment = Com.Microsoft.Azure.Mobile.Crashes.Model.ErrorAttachment;
    using AndroidErrorReport = Com.Microsoft.Azure.Mobile.Crashes.Model.ErrorReport;
    using AndroidICrashListener = Com.Microsoft.Azure.Mobile.Crashes.ICrashesListener;

    class AndroidCrashListener : Java.Lang.Object, AndroidICrashListener
    {
        private readonly PlatformCrashes _owner;

        public AndroidCrashListener(PlatformCrashes owner)
        {
            _owner = owner;
        }

        public AndroidErrorAttachment GetErrorAttachment(AndroidErrorReport androidReport)
        {
            var report = new ErrorReport(androidReport);
            var attachment = _owner.GetErrorAttachment(report);
            return attachment.ToAndroidErrorAttachment();
        }

        public void OnBeforeSending(AndroidErrorReport androidReport)
        {
            var report = new ErrorReport(androidReport);
            var e = new  SendingErrorReportEventArgs();
            e.Report = report;
            _owner.SendingErrorReport(null, e);
        }

        public void OnSendingFailed(AndroidErrorReport androidReport, Java.Lang.Exception exception)
        {
            var report = new ErrorReport(androidReport);
            var e = new FailedToSendErrorReportEventArgs();
            e.Report = report;
            e.Exception = exception;
            _owner.FailedToSendErrorReport(null, e);        
        }

        public void OnSendingSucceeded(AndroidErrorReport androidReport)
        {
            var report = new ErrorReport(androidReport);
            var e = new SentErrorReportEventArgs();
            e.Report = report;
            _owner.SentErrorReport(null, e);
        }

        public bool ShouldAwaitUserConfirmation()
        {
            return false;
        }

        public bool ShouldProcess(AndroidErrorReport androidReport)
        {
            var report = new ErrorReport(androidReport);
            return _owner.ShouldProcessErrorReport(report);
        }
    }
}
