using System;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models.Serialization;
using Microsoft.Azure.Mobile.Utils.Synchronization;

namespace Microsoft.Azure.Mobile.Rum
{
    public partial class RealUserMeasurements : MobileCenterService
    {
        #region static
        private static readonly object PushLock = new object();

        private static RealUserMeasurements _instanceField;

        public static RealUserMeasurements Instance
        {
            get
            {
                lock (PushLock)
                {
                    return _instanceField ?? (_instanceField = new RealUserMeasurements());
                }
            }
            set
            {
                lock (PushLock)
                {
                    _instanceField = value;
                }
            }
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            lock (PushLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (PushLock)
            {
                Instance.InstanceEnabled = enabled;
                return Task.FromResult(default(object));
            }
        }

        static void PlatformSetRumKey(string rumKey)
        {
        }

        static void PlatformSetConfigurationUrl(string url)
        {
        }

        #endregion

        #region instance

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }

            set
            {
                lock (_serviceLock)
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    if (value != prevValue)
                    {
                        ApplyEnabledState(value);
                    }
                }
            }
        }

        public override string ServiceName => "RealUserMeasurements";

        protected override string ChannelName => "rum";

        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            lock (_serviceLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);
            }
        }


        private void ApplyEnabledState(bool enabled)
        {
            lock (_serviceLock)
            {
                if (enabled && ChannelGroup != null)
                {
                    // TODO
                }
            }
        }
    }
}
