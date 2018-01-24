using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Channel
{
    /// <summary>
    /// IChannels are responsible for processing logs. In most cases, that means storing them and sending them to Ingestion.
    /// </summary>
    public interface IChannel : IDisposable
    {
        /// <summary>
        /// Invoked when a log will be enqueued.
        /// </summary>
        event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;

        /// <summary>
        /// Invoked when a log is about to filtered out or not.
        /// </summary>
        event EventHandler<FilteringLogEventArgs> FilteringLog;

        /// <summary>
        /// Invoke when a log is about to be sent.
        /// </summary>
        event EventHandler<SendingLogEventArgs> SendingLog;

        /// <summary>
        /// Invoked when a log successfully sent.
        /// </summary>
        event EventHandler<SentLogEventArgs> SentLog;

        /// <summary>
        /// Invoked when a log failed to send properly.
        /// </summary>
        event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;

        /// <summary>
        /// Enable or disable the channel
        /// </summary>
        /// <param name="enabled">Value indicating whether channel should be enabled or disabled</param>
        void SetEnabled(bool enabled);

        /// <summary>
        /// Stop all calls in progress and deactivate this channel
        /// </summary>
        Task ShutdownAsync();
    }
}
