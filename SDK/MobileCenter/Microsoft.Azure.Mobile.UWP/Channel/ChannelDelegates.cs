namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public delegate void EnqueuingLogEventHandler(object sender, EnqueuingLogEventArgs e);
    public delegate void SendingLogEventHandler(object sender, SendingLogEventArgs e);
    public delegate void SentLogEventHandler(object sender, SentLogEventArgs e);
    public delegate void FailedToSendLogEventHandler(object sender, FailedToSendLogEventArgs e);
}
