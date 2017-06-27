#import <Foundation/Foundation.h>
#import "RNPushDelegate.h"

#if __has_include(<React/RCTEventDispatcher.h>)
#import <React/RCTEventDispatcher.h>
#else
#import "RCTEventDispatcher.h"
#endif

#import "RNPushUtils.h"


static NSString *ON_PUSH_NOTIFICATION_RECEIVED_EVENT = @"MobileCenterPushNotificationReceived";

@implementation RNPushDelegateBase

- (instancetype) init
{
    return self;
}

- (void)push:(MSPush *)push didReceivePushNotification:(MSPushNotification *)pushNotification {
    [self.bridge.eventDispatcher sendAppEventWithName:ON_PUSH_NOTIFICATION_RECEIVED_EVENT body:convertNotificationToJS(pushNotification)];
}

@end
