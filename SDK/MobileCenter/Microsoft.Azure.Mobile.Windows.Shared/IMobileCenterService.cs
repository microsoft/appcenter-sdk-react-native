using Microsoft.Azure.Mobile.Channel;

namespace Microsoft.Azure.Mobile
{
    public interface IMobileCenterService
    {
        string ServiceName { get; }
        bool InstanceEnabled { get; set; }
        void OnChannelGroupReady(IChannelGroup channelGroup);
    }
}
