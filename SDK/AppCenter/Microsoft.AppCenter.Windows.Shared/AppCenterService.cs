// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// Provides basic functionality for IAppCenterServices.
    /// </summary>
    public abstract class AppCenterService : IAppCenterService
    {
        private const string PreferenceKeySeparator = "_";
        private const string KeyEnabled = Constants.KeyPrefix + "ServiceEnabled";
        protected readonly object _serviceLock = new object();

        /// <summary>
        /// Application settings.
        /// </summary>
        protected virtual IApplicationSettings ApplicationSettings => AppCenter.Instance.ApplicationSettings;

        protected virtual INetworkStateAdapter NetworkStateAdapter => AppCenter.Instance.NetworkStateAdapter;

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
        public virtual string LogTag => AppCenterLog.LogTag + ServiceName;

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

        /// <summary>
        /// Gets or sets whether service is enabled
        /// </summary>
        public virtual bool InstanceEnabled
        {
            get
            {
                lock (_serviceLock)
                {
                    return ApplicationSettings.GetValue(EnabledPreferenceKey, true);
                }
            }
            set
            {
                lock (_serviceLock)
                {
                    var enabledString = value ? "enabled" : "disabled";
                    if (value && !AppCenter.IsEnabledAsync().Result)
                    {
                        AppCenterLog.Error(LogTag,
                            "The SDK is disabled. Set AppCenter.Enabled to 'true' before enabling a specific service.");
                        return;
                    }
                    if (value == InstanceEnabled)
                    {
                        AppCenterLog.Info(LogTag, $"{ServiceName} service has already been {enabledString}.");
                        return;
                    }
                    Channel?.SetEnabled(value);
                    ApplicationSettings.SetValue(EnabledPreferenceKey, value);
                    AppCenterLog.Info(LogTag, $"{ServiceName} service has been {enabledString}");
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
            if (channelGroup == null)
            {
                throw new ArgumentNullException(nameof(channelGroup));
            }
            lock (_serviceLock)
            {
                ChannelGroup = channelGroup;
                Channel = channelGroup.AddChannel(ChannelName, TriggerCount, TriggerInterval, TriggerMaxParallelRequests);
                Channel.SetEnabled(InstanceEnabled);
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
                        AppCenterLog.Error(AppCenterLog.LogTag,
                            $"{ServiceName} service not initialized; discarding calls.");
                        return true;
                    }

                    if (InstanceEnabled)
                    {
                        return false;
                    }

                    AppCenterLog.Info(AppCenterLog.LogTag,
                        $"{ServiceName} service not enabled; discarding calls.");
                    return true;
                }
            }
        }
    }
}
