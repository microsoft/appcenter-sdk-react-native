using Microsoft.AAppCenterAnalytics.Channel;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;
using Moq;

namespace Microsoft.AppCenter.Analytics.Test.Windows
{
    public class SessionTrackerFactory : ISessionTrackerFactory
    {
        public Mock<ISessionTracker> ReturningSessionTrackerMock = new Mock<ISessionTracker>();

        public ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel, IApplicationSettings applicationSettings)
        {
            return ReturningSessionTrackerMock.Object;
        }
    }
}
