using System;

namespace Microsoft.Azure.Mobile.Push
{
	public partial class Push
	{
        private static bool PlatformEnabled { get; set; }
        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
