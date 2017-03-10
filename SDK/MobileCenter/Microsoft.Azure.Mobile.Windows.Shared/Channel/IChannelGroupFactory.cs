using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret);
    }
}
