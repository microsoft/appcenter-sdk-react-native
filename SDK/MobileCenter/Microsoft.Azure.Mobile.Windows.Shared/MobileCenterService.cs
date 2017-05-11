using System;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// Provides basic functionality for IMobileCenterServices.
    /// </summary>
    public abstract class MobileCenterService : IMobileCenterService
    {
        private const string PreferenceKeySeparator = "_";
        private const string KeyEnabled = Constants.KeyPrefix + "ServiceEnabled";
        private readonly object _serviceLock = new object();
        private readonly IApplicationSettings _applicationSettings = new ApplicationSettings();

        /// <summary>
        /// Channel associated with this service. Should be disposed only by ChannelGroup.
        /// </summary>
        protected IChannelUnit Channel { get; private set; }

        /// <summary>
        /// ChannelGroup that contains the service's ChannelUnit.
        /// </summary>
        protected IChannelGroup ChannelGroup { get; private set; }

        /// <summary>
        /// Name of this service's Channel.
        /// </summary>
        protected abstract string ChannelName { get; }
        
        /// <summary>
        /// Display name of the service.
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        /// Log tag for this service.
        /// </summary>
        public virtual string LogTag => MobileCenterLog.LogTag + ServiceName;

        /// <summary>
        /// Settings dictionary key for whether this service is enabled.
        /// </summary>
        protected virtual string EnabledPreferenceKey => KeyEnabled + PreferenceKeySeparator + ChannelName;

        /// <summary>
        /// Number of logs to enqueue before sending them to ingestion.
        /// </summary>
        protected virtual int TriggerCount => Constants.DefaultTriggerCount;

        /// <summary>
        /// Maximum time span to wait before triggering ingestion.
        /// </summary>
        protected virtual TimeSpan TriggerInterval => Constants.DefaultTriggerInterval;

        /// <summary>
        /// Maximum number of batches to process in parallel.
        /// </summary>
        protected virtual int TriggerMaxParallelRequests => Constants.DefaultTriggerMaxParallelRequests;

        protected MobileCenterService()
        {
        }

        // This constructor is only for testing
        internal MobileCenterService(IApplicationSettings settings)
        {
            _applicationSettings = settings;
        }

        /// <summary>
        /// Gets or sets whether service is enabled
        /// </summary>
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

        /// <summary>
        /// Method that is called to signal start of service.
        /// </summary>
        /// <param name="channelGroup">The channel group to which the channel should be added</param>
        /// <param name="appSecret">The app secret of the current application</param>

        public virtual void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            lock (_serviceLock)
            {
                ChannelGroup = channelGroup;
                Channel = channelGroup.AddChannel(ChannelName, TriggerCount, TriggerInterval, TriggerMaxParallelRequests);
                var enabled = MobileCenter.Enabled && InstanceEnabled;
                _applicationSettings[EnabledPreferenceKey] = enabled;
                Channel.SetEnabled(enabled);
            }
        }

        protected bool IsInactive
        {
            get
            {
                lock (_serviceLock)
                {
                    if (Channel == null)
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
