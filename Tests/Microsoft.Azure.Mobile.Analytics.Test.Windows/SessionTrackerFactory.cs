using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Channel;
using Moq;
using VSMC.Analytics.Windows.Shared;

namespace Microsoft.Azure.Mobile.Analytics.Test.Windows
{
    public class SessionTrackerFactory : ISessionTrackerFactory
    {
        public Mock<SessionTracker> ReturningSessionTrackerMock = new Mock<SessionTracker>();

        public SessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel)
        {
            return ReturningSessionTrackerMock.Object;
        }
    }
}
