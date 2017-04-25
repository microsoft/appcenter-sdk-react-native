using System;
using Foundation;
using ObjCRuntime;

namespace Microsoft.Azure.Mobile.Push.iOS.Bindings
{
	// @interface MSPush : MSServiceAbstract
	[BaseType(typeof(NSObject))]
	interface MSPush
	{
        // +(void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken;
		[Static]
        [Export("didRegisterForRemoteNotificationsWithDeviceToken:")]
		void DidRegisterForRemoteNotificationsWithDeviceToken(NSData deviceToken);

        // + (void)didFailToRegisterForRemoteNotificationsWithError:(NSError*)error;
        [Static]
        [Export("didFailToRegisterForRemoteNotificationsWithError:")]
        void DidFailToRegisterForRemoteNotificationsWithError(NSError error);

        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();
    }
}
