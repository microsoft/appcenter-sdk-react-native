using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Determine whether a particular error report should be processed.
    /// </summary>
    /// <param name="report">The error report that is being considered for processing.</param>
    /// <returns><c>true</c> if <c>report</c> should be processed.</returns>
    public delegate bool ShouldProcessErrorReportCallback(ErrorReport report);

    /// <summary>
    /// Get an error attachment for a particular error report.
    /// </summary>
    /// <param name="report">The error report for which an error attachment is to be returned.</param>
    /// <returns>The error attachment to be associated with <c>report</c>.</returns>
    public delegate ErrorAttachment GetErrorAttachmentCallback(ErrorReport report);

    /// <summary>
    /// Handler type for event <see cref="Crashes.SendingErrorReport"/>.
    /// </summary>
    /// <param name="sender">This parameter will be <c>null</c> when being sent from the <see cref="Crashes"/> class and should be ignored. </param>
    /// <param name="e">Event arguments. See <see cref="SendingErrorReportEventArgs"/> for more details.</param>
    public delegate void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e);

    /// <summary>
    /// Handler type for event <see cref="Crashes.SentErrorReport"/>.
    /// </summary>
    /// <param name="sender">This parameter will be <c>null</c> when being sent from the <see cref="Crashes"/> class and should be ignored. </param>
    /// <param name="e">Event arguments. See <see cref="SentErrorReportEventArgs"/> for more details.</param>
    public delegate void SentErrorReportHandler(object sender, SentErrorReportEventArgs e);

    /// <summary>
    /// Handler type for event <see cref="Crashes.FailedToSendErrorReport"/>.
    /// </summary>
    /// <param name="sender">This parameter will be <c>null</c> when being sent from the <see cref="Crashes"/> class and should be ignored. </param>
    /// <param name="e">Event arguments. See <see cref="FailedToSendErrorReportEventArgs"/> for more details.</param>
    public delegate void FailedToSendErrorHandler(object sender, FailedToSendErrorReportEventArgs e);
}
