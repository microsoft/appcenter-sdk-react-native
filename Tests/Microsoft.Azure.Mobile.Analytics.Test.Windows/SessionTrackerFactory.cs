using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using Moq;

namespace Microsoft.Azure.Mobile.Analytics.Test.Windows
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
