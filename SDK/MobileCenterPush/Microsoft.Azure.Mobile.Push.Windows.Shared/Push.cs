using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models.Serialization;
using Microsoft.Azure.Mobile.Push.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {
        #region static
        private static readonly object PushLock = new object();

        private static Push _instanceField;

        public static Push Instance
        {
            get
            {
                lock (PushLock)
                {
                    return _instanceField ?? (_instanceField = new Push());
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

        #endregion

        #region instance

        private readonly StatefulMutex _mutex = new StatefulMutex();

        public override string ServiceName => "Push";

        protected override string ChannelName => "push";

        public Push()
        {
            LogSerializer.AddLogType(PushInstallationLog.JsonIdentifier, typeof(PushInstallationLog));
        }

        /// <summary>
        /// Method that is called to signal start of the Push service.
        /// </summary>
        /// <param name="channelGroup"></param>
        /// <param name="appSecret"></param>
        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            using (_mutex.GetLock())
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(IsEnabledAsync().Result);
            }
        }

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }

            set
            {
                using (_mutex.GetLock())
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    if (value != prevValue)
                    {
                        _mutex.InvalidateState();
                        ApplyEnabledState(value);
                    }
                }
            }
        }

        #endregion
    }
}
