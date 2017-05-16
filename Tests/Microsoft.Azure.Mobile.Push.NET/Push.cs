using System;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {

        private void InstanceRegister()
        {
        }

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
