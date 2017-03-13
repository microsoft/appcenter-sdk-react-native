using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannelGroup : IChannel
    {
        void AddChannel(IChannel channel);
        IChannel AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches);
        void SetServerUrl(string serverUrl);
    }
}
