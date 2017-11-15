namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// User confirmation options for whether to send crash reports.
    /// </summary>
    public enum UserConfirmation
    {
        /// <summary>
        /// Do not send the crash report
        /// </summary>
        DontSend,

        /// <summary>
        /// Send the crash report
        /// </summary>
        Send,

        /// <summary>
        /// Send all crash reports
        /// </summary>
        AlwaysSend
    }
}
