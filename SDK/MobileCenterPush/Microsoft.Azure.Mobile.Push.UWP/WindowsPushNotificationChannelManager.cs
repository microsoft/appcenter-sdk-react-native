using Windows.Foundation;
using Windows.Networking.PushNotifications;

namespace Microsoft.Azure.Mobile.Push
{
    interface IWindowsPushNotificationChannelManager
    {
        IAsyncOperation<PushNotificationChannel> CreatePushNotificationChannelForApplicationAsync();
    }

    class WindowsPushNotificationChannelManager
    {
        public IAsyncOperation<PushNotificationChannel> CreatePushNotificationChannelForApplicationAsync()
        {
            return PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
        }
    }
}
