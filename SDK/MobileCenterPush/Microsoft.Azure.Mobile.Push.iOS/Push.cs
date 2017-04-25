using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Push
{
    using iOSPush = iOS.Bindings.MSPush;
    public partial class Push
    {
        static Type _internalBindingType = typeof(iOSPush);

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
    }
}
