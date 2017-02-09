using Microsoft.Azure.Mobile.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class Crashes : IMobileCenterService
    {
        public bool InstanceEnabled { get; set; }
        public void OnChannelGroupReady(ChannelGroup channelGroup)
        {

        }
    }
}
