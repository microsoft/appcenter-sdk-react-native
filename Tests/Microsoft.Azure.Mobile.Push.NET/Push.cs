using System;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {
        private void ApplyEnabledState()
        {
        }

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
