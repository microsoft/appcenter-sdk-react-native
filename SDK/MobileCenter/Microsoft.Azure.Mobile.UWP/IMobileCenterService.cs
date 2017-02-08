using Microsoft.Azure.Mobile.UWP.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP
{
    public interface IMobileCenterService
    {
        bool InstanceEnabled { get; set; }
        void OnChannelGroupReady(ChannelGroup channelGroup); //TODO or should this be IChannel?

        //TODO application lifecycle
    }
}
