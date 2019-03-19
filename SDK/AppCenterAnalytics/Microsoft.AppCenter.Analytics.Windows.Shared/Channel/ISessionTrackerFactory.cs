// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter.Analytics.Channel
{
    public interface ISessionTrackerFactory
    {
        ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel, IApplicationSettings applicationSettings);
    }
}
