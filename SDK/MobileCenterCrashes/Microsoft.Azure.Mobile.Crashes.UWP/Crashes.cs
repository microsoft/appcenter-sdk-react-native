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
        private static Crashes _instanceField;
        private static readonly object CrashesLock = new object();
        public static Crashes Instance
        {
            get
            {
                lock (CrashesLock)
                {
                    return _instanceField ?? (_instanceField = new Crashes());
                }
            }
            set
            {
                lock (CrashesLock)
                {
                    _instanceField = value; //for testing
                }
            }
        }

        protected override string ChannelName => "crashes";
        public override string ServiceName => "Crashes";

        /// <exception cref="MobileCenterException">Failed to register crashes with Watson</exception>
        public override void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
            var platformCrashes = PlatformCrashes as PlatformCrashes;
            platformCrashes?.Configure(channelGroup.AppSecret);
        }
    }
}
