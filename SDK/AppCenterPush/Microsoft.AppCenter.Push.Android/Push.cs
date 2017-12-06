using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Com.Microsoft.Appcenter.Push;

namespace Microsoft.AppCenter.Push
{
    public partial class Push
    {
        static Android.PushListener _pushListener = new Android.PushListener();

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
            var future = AndroidPush.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidPush.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        /// <summary>
        /// Sets the Sender ID necessary to receive push notifications.
        /// </summary>
        /// <value>
        /// The Sender ID to set.
        /// </value>
        public static void SetSenderId(string senderId)
        {
            AndroidPush.SetSenderId(senderId);
        }

        /// <summary>
        /// Call this before starting Push if you are using Firebase and want to use Firebase Analytics as well. If
        /// App Center detects Firebase, the default behavior is to disable it.
        /// </summary>
        public static void EnableFirebaseAnalytics()
        {
            AndroidPush.EnableFirebaseAnalytics(Application.Context);
        }

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidPush);

        /// <summary>
        /// If you are using the event for background push notifications
        /// and your activity has a launch mode such as singleTop, singleInstance or singleTask,
        /// need to call this method in your launcher OnNewIntent override method.
        /// </summary>
        /// <param name="activity">This activity.</param>
        /// <param name="intent">Intent from OnNewIntent().</param>
        public static void CheckLaunchedFromNotification(Activity activity, Intent intent)
        {
            AndroidPush.CheckLaunchedFromNotification(activity, intent);
        }
    }
}
