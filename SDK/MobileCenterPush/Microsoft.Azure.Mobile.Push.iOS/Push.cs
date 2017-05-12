using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.Azure.Mobile.Push.iOS;
using Microsoft.Azure.Mobile.Push.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Push
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
                    CustomData = NSDictionaryToDotNet(notification.CustomData)
                };
                PlatformPushNotificationReceived?.Invoke(null, pushEventArgs);
            };
            MSPush.SetDelegate(_pushDelegate);
        }

        private static bool PlatformEnabled
        {
            get { return MSPush.IsEnabled(); }
            set { MSPush.SetEnabled(value); }  
        }

        [Preserve]
        public static Type BindingType
        {
        	get
            {
                return _internalBindingType;
            }
        }

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;

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

        private static IDictionary<string, string> NSDictionaryToDotNet(NSDictionary<NSString, NSString> nsdict)
        {
            var dict = new Dictionary<string, string>();
            foreach (var key in nsdict.Keys)
            {
                dict[key.ToString()] = nsdict.ObjectForKey(key);
            }
            return dict;
        }
    }
}
