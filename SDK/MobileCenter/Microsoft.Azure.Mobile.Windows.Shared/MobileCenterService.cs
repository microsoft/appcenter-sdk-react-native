using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;

//TODO thread safety

namespace Microsoft.Azure.Mobile
{
    public abstract class MobileCenterService : IMobileCenterService
    {
        private const string PreferenceKeySeparator = "_";
        private const int DefaultTriggerCount = 50;
        private static readonly TimeSpan DefaultTriggerInterval = TimeSpan.FromSeconds(3);
        private const int DefaultTriggerMaxParallelRequests = 3;
        private const string KeyEnabled = "MobileCenterServiceEnabled";

        private readonly IApplicationSettings _applicationSettings = new ApplicationSettings();
        protected ChannelGroup ChannelGroup { get; private set; }

        protected abstract string ChannelName { get; }
        protected abstract string ServiceName { get; }
        public virtual string LogTag => MobileCenterLog.LogTag + ServiceName;
        protected virtual string EnabledPreferenceKey => KeyEnabled + PreferenceKeySeparator + ChannelName;
        protected virtual int TriggerCount => DefaultTriggerCount;
        protected virtual TimeSpan TriggerInterval => DefaultTriggerInterval;
        protected virtual int TriggerMaxParallelRequests => DefaultTriggerMaxParallelRequests;

        public virtual bool InstanceEnabled
        {
            get
            {
                return _applicationSettings.GetValue(KeyEnabled, true);
            }
            set
            {
                var enabledString = (value ? "enabled" : "disabled");
                if (value && !MobileCenter.Enabled)
                {
                    MobileCenterLog.Error(LogTag, 
                        "The SDK is disabled. Set MobileCenter.Enabled to 'true' before enabling a specific service.");
                    return;
                }
                if (value == InstanceEnabled)
                {
                    MobileCenterLog.Info(LogTag, $"{ServiceName} service has already been {enabledString}.");
                    return;
                }
                if (ChannelGroup != null)
                {
                    var channel = ChannelGroup.GetChannel(ChannelName);
                    channel.Enabled = value;
                    if (!value)
                    {
                        channel.Clear();
                    }
                }
                _applicationSettings[KeyEnabled] = value;
                MobileCenterLog.Info(LogTag, $"{ServiceName} service has been {enabledString}");
            }
        }

        public virtual void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            ChannelGroup = channelGroup;
            ChannelGroup.AddChannel(ChannelName, TriggerCount, TriggerInterval, TriggerMaxParallelRequests);
            ChannelGroup.GetChannel(ChannelName).Enabled = InstanceEnabled;
        }
    }
}
