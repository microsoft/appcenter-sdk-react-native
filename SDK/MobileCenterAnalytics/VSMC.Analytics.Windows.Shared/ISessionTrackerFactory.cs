using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Channel;

namespace VSMC.Analytics.Windows.Shared
{
    public interface ISessionTrackerFactory
    {
        SessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel);
    }
}
