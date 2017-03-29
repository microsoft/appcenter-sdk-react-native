using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Channel;

namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    public interface ISessionTrackerFactory
    {
        ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel);
    }
}
