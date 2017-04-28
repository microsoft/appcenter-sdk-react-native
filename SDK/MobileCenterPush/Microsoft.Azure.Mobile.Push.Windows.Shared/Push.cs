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
        public static bool Enabled
        {
            get
            {
                return Instance.InstanceEnabled;
            }
            set
            {
                Instance.InstanceEnabled = value;
            }
        }

        /// <summary>
        /// Retrieve the push token from platform-specific Push Notification Service,
        /// and later use the token to register with Mobile Center backend.
        /// </summary>
        public static void Register()
        {
            Instance.InstanceRegister();
        }

        #endregion

        #region instance

        private readonly StatefulMutex _mutex;
        private readonly StateKeeper _stateKeeper = new StateKeeper();

        public override string ServiceName
        {
            get
            {
                return "Push";
            }
        }

        protected override string ChannelName
        {
            get
            {
                return "push";
            }
        }

        public Push()
        {
            _mutex = new StatefulMutex(_stateKeeper);

            LogSerializer.AddLogType(PushInstallationLog.JsonIdentifier, typeof(PushInstallationLog));
        }

        /// <summary>
        /// Method that is called to signal start of the Push service.
        /// </summary>
        /// <param name="channelGroup"></param>
        public override void OnChannelGroupReady(IChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
        }

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }

            set
            {
                base.InstanceEnabled = value;
            }
        }

        #endregion
    }
}
