using System;
using System.Threading.Tasks;
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
                PushNotificationReceived?.Invoke(null, pushEventArgs);
            };
            AndroidPush.SetListener(_pushListener);
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            var consumer = new Consumer<bool>();
            AndroidPush.IsEnabled().ThenAccept(consumer);
            return consumer.Task;
        }

        static void PlatformSetEnabled(bool enabled)
        {
            AndroidPush.SetEnabled(enabled);
        }

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidPush);

        public static void EnableFirebaseAnalytics()
        {
            AndroidPush.EnableFirebaseAnalytics(global::Android.App.Application.Context);
        }
    }
}
