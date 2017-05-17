using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils.Synchronization;

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

        /// <summary>
        /// Push module enabled or disabled
        /// </summary>
        private static bool PlatformEnabled
        {
            get
            {
                lock (PushLock)
                {
                    return Instance.InstanceEnabled;
                }
            }
            set
            {
                lock (PushLock)
                {
                    Instance.InstanceEnabled = value;
                }
            }
        }

        #endregion

        #region instance

        private readonly StatefulMutex _mutex;
        private readonly StateKeeper _stateKeeper = new StateKeeper();

        public override string ServiceName => "Push";

        protected override string ChannelName => "push";
        public Push()
        {
            _mutex = new StatefulMutex(_stateKeeper);
            LogSerializer.AddLogType(PushInstallationLog.JsonIdentifier, typeof(PushInstallationLog));
        }

        /// <summary>
        /// Method that is called to signal start of the Push service.
        /// </summary>
        /// <param name="channelGroup"></param>
        /// <param name="appSecret"></param>
        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            _mutex.Lock();
            try
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(Enabled);
            }
            finally
            {
                _mutex.Unlock();
            }
        }

        public override bool InstanceEnabled
        {
            get
            {
                _mutex.Lock();
                try
                {
                    return base.InstanceEnabled;
                }
                finally
                {
                    _mutex.Unlock();
                }
            }

            set
            {
                _mutex.Lock();
                try
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    _stateKeeper.InvalidateState();
                    if (value != prevValue)
                    {
                        ApplyEnabledState(value);
                    }
                }
                finally
                {
                    _mutex.Unlock();
                }
            }
        }

        #endregion
    }
}
