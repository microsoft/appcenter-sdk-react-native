using System;
using Windows.ApplicationModel.Activation;
using Microsoft.Azure.Mobile.Channel;
#if REFERENCE
#else
using WatsonRegistrationUtility;
#endif

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class Crashes : IMobileCenterService
    {
        public void NotifyOnLaunched(LaunchActivatedEventArgs e)
        {
            // Nothing to do.
        }

        public string ServiceName => "Crashes";

        public bool InstanceEnabled { get; set; }

        private static Crashes _instanceField;

        public static Crashes Instance
        {
            get
            {
                return _instanceField ?? (_instanceField = new Crashes());
            }
            set
            {
                _instanceField = value; //for testing
            }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            MobileCenterLog.Warn(MobileCenterLog.LogTag, "Crashes service is not yet supported on UWP.");
            try
            {
#if REFERENCE
#else
                WatsonRegistrationManager.Start(appSecret);
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                throw new MobileCenterException("Failed to register crashes with Watson", e);
#endif
            }
        }
    }
}
