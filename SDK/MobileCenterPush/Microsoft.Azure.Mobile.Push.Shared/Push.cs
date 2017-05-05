using System;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push
    {
        /// <summary>
        /// Enable or disable Push module.
        /// </summary>
        public static bool Enabled
        {
            get { return PlatformEnabled; }
            set { PlatformEnabled = value; }
        }

        /// <summary>
        /// Occurs when the application receives a push notification.
        /// </summary>
        public static event EventHandler<PushNotificationReceivedEventArgs> PushNotificationReceived
        {
            add
            {
                PlatformPushNotificationReceived += value;
            }
            remove
            {
                PlatformPushNotificationReceived -= value;
            }
        }
    }
}
