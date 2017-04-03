using System;
using Com.Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Event arguments base class for all events that involve an error report.
    /// </summary>
    public class ErrorReportEventArgs : NetworkStateHelper.EventArgs
    {
        /// <summary>
        /// The error report associated with the event.
        /// </summary>
        public ErrorReport Report;
    }

    /// <summary>
    /// Event arguments class for event <see cref="Crashes.SendingErrorReport"/>.
    /// </summary>
    public class SendingErrorReportEventArgs : ErrorReportEventArgs { }

    /// <summary>
    /// Event arguments class for event <see cref="Crashes.SentErrorReport"/>.
    /// </summary>
    public class SentErrorReportEventArgs : ErrorReportEventArgs { }

    /// <summary>
    /// Event arguments class for event <see cref="Crashes.FailedToSendErrorReport"/>.
    /// </summary>
    public class FailedToSendErrorReportEventArgs : ErrorReportEventArgs
    {
        /// <summary>
        /// The native exception associated with the failure.
        /// </summary>
        public object Exception;
    }
}
