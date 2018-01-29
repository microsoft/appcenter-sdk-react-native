using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Channel;
using System;
using System.Threading.Tasks;

namespace Contoso.Forms.Puppet.UWP
{
    public class EventFilter : AppCenterService
    {
        #region static

        private static readonly object EventFilterLock = new object();

        private static EventFilter _instanceField;

        public static EventFilter Instance
        {
            get
            {
                lock (EventFilterLock)
                {
                    return _instanceField ?? (_instanceField = new EventFilter());
                }
            }
            set
            {
                lock (EventFilterLock)
                {
                    _instanceField = value; //for testing
                }
            }
        }

        /// <summary>
        /// Check whether the EventFilter service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            lock (EventFilterLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        /// <summary>
        /// Enable or disable the EventFilter service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            lock (EventFilterLock)
            {
                Instance.InstanceEnabled = enabled;
                return Task.FromResult(default(object));
            }
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

        protected override string ChannelName => "event_filter";

        public override string ServiceName => "EventFilter";

        public Type BindingType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
                if (ChannelGroup != null)
                {
                    if (enabled)
                    {
                        ChannelGroup.FilteringLog += FilterLog;
                    }
                    else
                    {
                        ChannelGroup.FilteringLog -= FilterLog;
                    }
                }
            }
        }

        private void FilterLog(object sender, FilteringLogEventArgs e)
        {
            if (e.Log is EventLog)
            {
                e.FilterRequested = true;
            }
        }

        #endregion
    }
}
