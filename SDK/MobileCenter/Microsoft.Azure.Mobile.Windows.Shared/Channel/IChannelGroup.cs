using System;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannelGroup : IChannel
    {
        void AddChannel(IChannelUnit channel);
        IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches);
        void SetServerUrl(string serverUrl);
    }
}
