using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Com.Microsoft.Azure.Mobile.Push;

namespace Microsoft.Azure.Mobile.Push
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

        static bool PlatformEnabled
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

        /// <summary>
        /// Enables firebase analytics collection.
        /// It's disabled by default unless you call this method.
        /// </summary>
        public static void EnableFirebaseAnalytics()
        {
            AndroidPush.EnableFirebaseAnalytics(Application.Context);
        }

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
