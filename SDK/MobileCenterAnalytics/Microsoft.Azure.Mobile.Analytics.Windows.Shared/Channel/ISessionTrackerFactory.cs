using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    public interface ISessionTrackerFactory
    {
        ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel, IApplicationSettings applicationSettings);
    }
}
