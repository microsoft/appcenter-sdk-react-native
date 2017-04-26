using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;

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

        public static bool Enabled
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

        public static void Register()
        {
            lock (PushLock)
            {
                Instance.InstanceRegister();
            }
        }

        #endregion

        #region instance

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
            LogSerializer.AddLogType(PushInstallationLog.JsonIdentifier, typeof(PushInstallationLog));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelGroup"></param>
        public override void OnChannelGroupReady(IChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
            //ApplyEnabledState(InstanceEnabled);
        }

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }

            set
            {
                //bool prevValue = InstanceEnabled;
                base.InstanceEnabled = value;
                //if (value != prevValue)
                //{
                //    ApplyEnabledState(value);
                //}
            }
        }

        private void ApplyEnabledState(bool enabled)
        {
            if (enabled && ChannelGroup != null)
            {
                //
            }
            else if (!enabled)
            {
                //
            }
        }

        #endregion
    }
}
