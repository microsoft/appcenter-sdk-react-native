using System;
using Android.App;
using Com.Microsoft.AppCenter.Push;

namespace Microsoft.AppCenter.Push.Android
{
    public class PushListener : Java.Lang.Object, IPushListener
    {
        public void OnPushNotificationReceived(Activity activity, AndroidPushNotification notification)
        {
            OnPushNotificationReceivedAction?.Invoke(notification);
        }

        public Action<AndroidPushNotification> OnPushNotificationReceivedAction { get; set; }
    }
}
