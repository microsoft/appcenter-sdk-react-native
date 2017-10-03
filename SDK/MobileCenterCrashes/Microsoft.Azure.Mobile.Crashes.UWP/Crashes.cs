using System;
using Microsoft.Azure.Mobile.Channel;
#if REFERENCE
#else
using WatsonRegistrationUtility;
#endif

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class Crashes : IMobileCenterService
    {
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
                MobileCenter.CorrelationIdChanged += (s, id) =>
                {
                    // Checking for null and setting id needs to be atomic to avoid
                    // overwriting. But only do that if the id argument is null to avoid
                    // needlessly waiting on the lock.
                    if (id == null &&
                        MobileCenter.TestAndSetCorrelationId(null, Guid.NewGuid().ToString()))
                    {
                        // Return here to avoid setting the correlation id twice.
                        return;
                    }
                    WatsonRegistrationManager.SetCorrelationId(id);
                };

                // Checking for null and setting id needs to be atomic to avoid
                // overwriting
                MobileCenter.TestAndSetCorrelationId(null, Guid.NewGuid().ToString());
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
