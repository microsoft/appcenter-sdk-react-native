#import <AppCenterPush/AppCenterPush.h>
#import "AppCenterReactNativePushDelegate.h"

#if __has_include(<React/RCTEventDispatcher.h>)
#import <React/RCTEventDispatcher.h>
#else
#import "RCTEventDispatcher.h"
#endif

#import "AppCenterReactNativePushUtils.h"


static NSString *ON_PUSH_NOTIFICATION_RECEIVED_EVENT = @"AppCenterPushNotificationReceived";

@implementation AppCenterReactNativePushDelegateBase
{
    bool hasListeners;
}

- (instancetype) init
{
    self.saveInitialNotification = true;
    self.initialNotification = nil;
    return self;
}

- (void)push:(MSPush *)push didReceivePushNotification:(MSPushNotification *)pushNotification {
    // If saveInitialNotification is true, assume we've just been launched and save the first notification we receive.
    // This handles the scenario that when the user taps on a background notification to launch the app, the launch notification
    // gets sent to this native callback before the JS callback has a chance to register. So we need to save that notification off,
    // then send it when the JS callback regsters & stop saving notifications after
    if (self.saveInitialNotification && self.initialNotification == nil) {
        self.initialNotification = convertNotificationToJS(pushNotification);
    }
    else if (hasListeners) {
        [self.eventEmitter sendEventWithName:ON_PUSH_NOTIFICATION_RECEIVED_EVENT body:convertNotificationToJS(pushNotification)];
    }
}

- (void)sendAndClearInitialNotification
{
    if (self.initialNotification) {
        if (hasListeners) {
            [self.eventEmitter sendEventWithName:ON_PUSH_NOTIFICATION_RECEIVED_EVENT body:self.initialNotification];
        }
        self.initialNotification = nil;
    }
    self.saveInitialNotification = false;
}

- (NSArray<NSString *> *)supportedEvents
{
    return @[ON_PUSH_NOTIFICATION_RECEIVED_EVENT];
}

- (void)startObserving {
    hasListeners = YES;
}

- (void)stopObserving {
    hasListeners = NO;
}

@end
