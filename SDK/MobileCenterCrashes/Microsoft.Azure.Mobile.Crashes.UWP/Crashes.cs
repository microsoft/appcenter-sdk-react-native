using Microsoft.Azure.Mobile.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class Crashes
    {
        protected override string ChannelName => "crashes";
        protected override string ServiceName => "Crashes";
    }
}
