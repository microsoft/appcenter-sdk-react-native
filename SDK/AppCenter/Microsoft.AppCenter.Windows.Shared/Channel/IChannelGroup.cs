using System;

namespace Microsoft.AppCenter.Channel
{
    /// <summary>
    /// Represents a collection of channels that can perform aggregate operations
    /// </summary>
    public interface IChannelGroup : IChannel
    {
        /// <summary>
        /// Adds an IChannelUnit to the group. This transfers ownership of the IChannelUnit to IChannelGroup.
        /// </summary>
        /// <param name="channel">The ChannelUnit to add</param>
        void AddChannel(IChannelUnit channel);

       /// <summary>
       /// Adds an IChannelUnit to the group.
       /// </summary>
       /// <param name="name">The name of the ChannelUnit</param>
       /// <param name="maxLogsPerBatch">The maximum batch size for the ChannelUnit</param>
       /// <param name="batchTimeInterval">The maximum time interval between batches</param>
       /// <param name="maxParallelBatches">The maximum number of batches to be processed in parallel</param>
       /// <returns>The created IChannelUnit</returns>
        IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches);

        /// <summary>
        /// Sets the ingestion endpoint to send App Center logs to.
        /// </summary>
        /// <param name="logUrl">The log URL</param>
        void SetLogUrl(string logUrl);
    }
}
