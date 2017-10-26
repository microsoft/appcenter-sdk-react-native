using System;
using Microsoft.AppCenter.Channel;
#if REFERENCE
#else
using WatsonRegistrationUtility;
#endif

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes : IAppCenterService
    {
        public string ServiceName => "Crashes";

        /// <summary>
        /// This property does not return a meaningful value on Windows.
        /// </summary>
        public bool InstanceEnabled { get; set; }

        private static Crashes _instanceField;

        public static Crashes Instance
        {
            get
            {
                return _instanceField ?? (_instanceField = new Crashes());
            }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Crashes service is not yet supported on this platform.");
            try
            {
#if REFERENCE
#else
                WatsonRegistrationManager.Start(appSecret);
#pragma warning disable CS0612 // Type or member is obsolete
                AppCenter.CorrelationIdChanged += (s, id) =>
                {
                    WatsonRegistrationManager.SetCorrelationId(id.ToString());
                };

                // Checking for null and setting id needs to be atomic to avoid
                // overwriting
                Guid newId = Guid.NewGuid();
                AppCenter.TestAndSetCorrelationId(Guid.Empty, ref newId);
#pragma warning restore CS0612 // Type or member is obsolete
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                throw new AppCenterException("Failed to register crashes with Watson", e);
#endif
            }
        }
    }
}
