using System.Collections.Generic;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Callback type for determining whether a particular error report should be processed.
    /// </summary>
    /// <returns><c>true</c> if <c>report</c> should be processed.</returns>
    /// <param name="report">The error report that is being considered for processing.</param>
    public delegate bool ShouldProcessErrorReportCallback(ErrorReport report);

    /// <summary>
    /// Callback type for getting error attachments for a particular error report.
    /// </summary>
    /// <returns>The error attachments to be associated with <c>report</c>.</returns>
    /// <param name="report">The error report for which error attachments are to be returned.</param>
    public delegate IEnumerable<ErrorAttachmentLog> GetErrorAttachmentsCallback(ErrorReport report);

    /// <summary>
    /// Determine whether user confirmation is required to process a report. <see cref="Crashes.NotifyUserConfirmation"/> must be called by yourself./> 
    /// </summary>
    /// <returns><c>true</c> if sending reports should be confirmed by a user.</returns>
    public delegate bool ShouldAwaitUserConfirmationCallback();

    /// <summary>
    /// Handler type for event <see cref="Crashes.SendingErrorReport"/>.
    /// </summary>
    /// <param name="sender">This parameter will be <c>null</c> when being sent from the <see cref="Crashes"/> class and should be ignored. </param>
    /// <param name="e">Event arguments. See <see cref="SendingErrorReportEventArgs"/> for more details.</param>
    public delegate void SendingErrorReportEventHandler(object sender, SendingErrorReportEventArgs e);

    /// <summary>
    /// Handler type for event <see cref="Crashes.SentErrorReport"/>.
    /// </summary>
    /// <param name="sender">This parameter will be <c>null</c> when being sent from the <see cref="Crashes"/> class and should be ignored. </param>
    /// <param name="e">Event arguments. See <see cref="SentErrorReportEventArgs"/> for more details.</param>
    public delegate void SentErrorReportEventHandler(object sender, SentErrorReportEventArgs e);

    /// <summary>
    /// Handler type for event <see cref="Crashes.FailedToSendErrorReport"/>.
    /// </summary>
    /// <param name="sender">This parameter will be <c>null</c> when being sent from the <see cref="Crashes"/> class and should be ignored. </param>
    /// <param name="e">Event arguments. See <see cref="FailedToSendErrorReportEventArgs"/> for more details.</param>
    public delegate void FailedToSendErrorReportEventHandler(object sender, FailedToSendErrorReportEventArgs e);
}
