using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.Azure.Mobile.Push.iOS;
using Microsoft.Azure.Mobile.Push.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Push
{
    using iOSPush = iOS.Bindings.MSPush;
    public partial class Push
    {
        static Type _internalBindingType = typeof(iOSPush);
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
            iOSPush.SetDelegate(_pushDelegate);
        }

        private static bool PlatformEnabled
        {
            get { return iOSPush.IsEnabled(); }
            set { iOSPush.SetEnabled(value); }  
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
            iOSPush.DidRegisterForRemoteNotificationsWithDeviceToken(deviceToken);
        }

        /// <summary>
        /// Call this from the corresponding method override in your AppDelegate.
        /// </summary>
        /// <param name="error">Associated error.</param>
        public static void FailedToRegisterForRemoteNotifications(NSError error)
        {
            iOSPush.DidFailToRegisterForRemoteNotificationsWithError(error);
        }

        private static IDictionary<string, string> NSDictionaryToDotNet(NSDictionary<NSString, NSString> nsdict)
        {
            var dict = new Dictionary<string, string>();

            return dict;
        }
    }
}
