using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public class ChannelGroup : IChannel
    {
        private Dictionary<string, Channel> Channels;

        #region Events
        public event EnqueuingLogEventHandler EnqueuingLog;
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;
        #endregion

        public void SetServerUrl(string serverUrl)
        {
            // set server url
        }
        public void AddChannel(Channel channel)
        {
            // add channel
        }
        public void RemoveChannel(Channel channel)
        {
            // remove channel 
        }

        public Channel GetChannel(string channelName)
        {
            // get channel
            return null;
        }

        public bool Enabled { get; set; }
    }
}
