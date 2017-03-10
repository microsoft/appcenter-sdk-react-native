using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
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
        private readonly object _serviceLock = new object();

        private readonly IApplicationSettings _applicationSettings = new ApplicationSettings();
        protected IChannelGroup ChannelGroup { get; private set; }
        protected IChannel Channel { get; private set; }
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
                lock (_serviceLock)
                {
                    return _applicationSettings.GetValue(EnabledPreferenceKey, true);
                }
            }
            set
            {
                lock (_serviceLock)
                {
                    var enabledString = value ? "enabled" : "disabled";
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
                    Channel?.SetEnabled(value);
                    _applicationSettings[EnabledPreferenceKey] = value;
                    MobileCenterLog.Info(LogTag, $"{ServiceName} service has been {enabledString}");
                }
            }
        }

        public virtual void OnChannelGroupReady(IChannelGroup channelGroup)
        {
            lock (_serviceLock)
            {
                ChannelGroup = channelGroup;
                Channel = ChannelGroup.AddChannel(ChannelName, TriggerCount, TriggerInterval, TriggerMaxParallelRequests);
                _applicationSettings[EnabledPreferenceKey] = MobileCenter.Enabled;
                Channel?.SetEnabled(MobileCenter.Enabled);
            }
        }

        protected bool IsInactive
        {
            get
            {
                lock (_serviceLock)
                {
                    if (ChannelGroup == null)
                    {
                        MobileCenterLog.Error(MobileCenterLog.LogTag,
                            $"{ServiceName} service not initialized; discarding calls.");
                        return true;
                    }

                    if (InstanceEnabled)
                    {
                        return false;
                    }

                    MobileCenterLog.Info(MobileCenterLog.LogTag,
                        $"{ServiceName} service not enabled; discarding calls.");
                    return true;
                }
            }
        }
    }
}
