using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Channel
{
    /// <summary>
    /// IChannels are responsible for processing logs. In most cases, that means storing them and sending them to Ingestion.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Invoked when a log will be enqueued
        /// </summary>
        event EnqueuingLogEventHandler EnqueuingLog;

        /// <summary>
        /// Invoke when a log will be sent
        /// </summary>
        event SendingLogEventHandler SendingLog;

        /// <summary>
        /// Invoked when a log successfully sent
        /// </summary>
        event SentLogEventHandler SentLog;

        /// <summary>
        /// Invoked when a log failed to send properly
        /// </summary>
        event FailedToSendLogEventHandler FailedToSendLog;

        /// <summary>
        /// Enable or disable the channel
        /// </summary>
        /// <param name="enabled">Value indicating whether channel should be enabled or disabled</param>
        void SetEnabled(bool enabled);

        /// <summary>
        /// Stop all calls in progress and deactivate this channel
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Enqueue a log for processing
        /// </summary>
        /// <param name="log"></param>
        void Enqueue(Log log);
    }
}
