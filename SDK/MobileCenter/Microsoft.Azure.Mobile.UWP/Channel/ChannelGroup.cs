using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public class ChannelGroup : IChannel
    {
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;
        public void Clear()
        {
            // clear logs of all child channels
        }
        public string ServerUrl { get; set; }

        //void AddGroup(string groupName, int maxLogsPerBatch, long batchTimeInterval /* should use timeinterval class instead of long? */, int maxParallelBatches /* what is this parameter */);
        public void AddChannel(Channel channel)
        {
            // add channel
        }
        public void RemoveChannel(Channel channel)
        {
            // remove channel 
        }

        //should this return IChannel or Channel?
        public IChannel GetChannel(string channelName)
        {
            // get channel
            return null;
        }

        public bool Enabled { get; set; }
    }
}
