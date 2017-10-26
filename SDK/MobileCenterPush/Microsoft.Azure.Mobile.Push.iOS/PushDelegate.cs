using System;
using Microsoft.AppCenter.Push.iOS.Bindings;

namespace Microsoft.AppCenter.Push.iOS
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
