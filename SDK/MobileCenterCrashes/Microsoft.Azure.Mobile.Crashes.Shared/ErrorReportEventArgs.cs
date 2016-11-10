using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class ErrorReportEventArgs : EventArgs
    {
        public ErrorReport Report;
    }

    public class SendingErrorReportEventArgs : ErrorReportEventArgs { }
    public class SentErrorReportEventArgs : ErrorReportEventArgs { }
    public class FailedToSendErrorReportEventArgs : ErrorReportEventArgs
    {
        public object Exception;
    }
}
