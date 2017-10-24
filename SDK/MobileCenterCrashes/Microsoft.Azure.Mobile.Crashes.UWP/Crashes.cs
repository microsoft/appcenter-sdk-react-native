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
#pragma warning disable CS0612 // Type or member is obsolete
                MobileCenter.CorrelationIdChanged += (s, id) =>
                {
                    WatsonRegistrationManager.SetCorrelationId(id.ToString());
                };

                // Checking for null and setting id needs to be atomic to avoid
                // overwriting
                Guid newId = Guid.NewGuid();
                MobileCenter.TestAndSetCorrelationId(Guid.Empty, ref newId);
#pragma warning restore CS0612 // Type or member is obsolete
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
