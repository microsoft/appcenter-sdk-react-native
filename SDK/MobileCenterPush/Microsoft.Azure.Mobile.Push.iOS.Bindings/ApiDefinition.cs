using System;
using Foundation;
using ObjCRuntime;

namespace Microsoft.AppCenter.Push.iOS.Bindings
{
    // @interface MSPush : MSServiceAbstract
    [BaseType(typeof(NSObject))]
    interface MSPush
    {
        // +(void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken;
        [Static]
        [Export("didRegisterForRemoteNotificationsWithDeviceToken:")]
        void DidRegisterForRemoteNotificationsWithDeviceToken(NSData deviceToken);

        // +(void)didFailToRegisterForRemoteNotificationsWithError:(NSError*)error;
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

        // +(void)setDelegate:(nullable id<MSPushDelegate>)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] MSPushDelegate _delegate);

        //+ (BOOL)didReceiveRemoteNotification:(NSDictionary *)userInfo;
        [Static]
        [Export("didReceiveRemoteNotification:")]
        bool DidReceiveRemoteNotification(NSDictionary userInfo);
    }

    // @protocol MSCrashesDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSPushDelegate
    {
        //@optional -(void)push:(MSPush *)push didReceivePushNotification:(MSPushNotification*)pushNotification;
        [Export("push:didReceivePushNotification:")]
        void ReceivedPushNotification(MSPush push, MSPushNotification pushNotification);
    }

    // @interface MSPushNotification : NSObject
    [BaseType(typeof(NSObject))]
    interface MSPushNotification
    {
        // @property(nonatomic, copy, readonly) NSString *title;
        [Export("title")]
        string Title { get; }

        //@property(nonatomic, copy, readonly) NSString *message;
        [Export("message")]
        string Message { get; }

        //@property(nonatomic, copy, readonly) NSDictionary<NSString*, NSString*> *customData;
        [Export("customData")]
        NSDictionary<NSString, NSString> CustomData { get; }
    }
}
