using System;
using Foundation;
using Microsoft.Azure.Mobile.Push.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Push.iOS
{
    public class PushDelegate : MSPushDelegate
    {
        public override void ReceivedPushNotification(MSPush push, MSPushNotification pushNotification)
        {
            OnPushNotificationReceivedAction?.Invoke(pushNotification);
        }

        public Action<MSPushNotification> OnPushNotificationReceivedAction { get; set; }
    }
}
