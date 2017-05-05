using System;
using Android.Runtime;
using Com.Microsoft.Azure.Mobile.Push;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push
    {
        private static Android.PushListener _pushListener = new Android.PushListener();
        static Push()
        {
            _pushListener.OnPushNotificationReceivedAction = notification =>
            {
                var pushEventArgs = new PushNotificationReceivedEventArgs
                {
                    Title = notification.Title,
                    Message = notification.Message,
                    CustomData = notification.CustomData
                };
                PlatformPushNotificationReceived?.Invoke(null, pushEventArgs);
            };
            AndroidPush.SetListener(_pushListener);
        }

        private static bool PlatformEnabled
        {
        	get { return AndroidPush.Enabled; }
        	set { AndroidPush.Enabled = value; }
        }

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidPush);

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;

        public static void EnableFirebaseAnalytics()
        {
            AndroidPush.EnableFirebaseAnalytics(global::Android.App.Application.Context);
        }
    }
}
