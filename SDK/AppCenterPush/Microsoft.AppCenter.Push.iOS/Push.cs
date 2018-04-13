using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Push.iOS;
using Microsoft.AppCenter.Push.iOS.Bindings;

namespace Microsoft.AppCenter.Push
{
    public partial class Push
    {
        static Type _internalBindingType = typeof(MSPush);
        static PushDelegate _pushDelegate = new PushDelegate();

        static Push()
        {
            _pushDelegate.OnPushNotificationReceivedAction = notification =>
            {
                var pushEventArgs = new PushNotificationReceivedEventArgs
                {
                    Title = notification.Title,
                    Message = notification.Message,
                    CustomData = notification.CustomData?.ToDictionary(i => i.Key.ToString(), i => i.Value.ToString())
                };
                PushNotificationReceived?.Invoke(null, pushEventArgs);
            };
            MSPush.SetDelegate(_pushDelegate);
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSPush.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSPush.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        [Preserve]
        public static Type BindingType
        {
            get
            {
                return _internalBindingType;
            }
        }

        /// <summary>
        /// Call this from the corresponding method override in your AppDelegate.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        public static void RegisteredForRemoteNotifications(NSData deviceToken)
        {
            MSPush.DidRegisterForRemoteNotificationsWithDeviceToken(deviceToken);
        }

        /// <summary>
        /// Call this from the corresponding method override in your AppDelegate.
        /// </summary>
        /// <param name="error">Associated error.</param>
        public static void FailedToRegisterForRemoteNotifications(NSError error)
        {
            MSPush.DidFailToRegisterForRemoteNotificationsWithError(error);
        }

        public static bool DidReceiveRemoteNotification(NSDictionary userInfo)
        {
            return MSPush.DidReceiveRemoteNotification(userInfo);
        }
    }
}
