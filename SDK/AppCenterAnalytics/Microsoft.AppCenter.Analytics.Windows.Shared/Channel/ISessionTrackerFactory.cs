using Microsoft.AppCenterChannel;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter.Analytics.Channel
{
    public interface ISessionTrackerFactory
    {
        ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel, IApplicationSettings applicationSettings);
    }
}
