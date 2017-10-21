#import "RNPush.h"

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTAssert.h>)
#import <React/RCTAssert.h>
#import <React/RCTBridgeModule.h>
#import <React/RCTConvert.h>
#import <React/RCTEventDispatcher.h>
#import <React/RCTRootView.h>
#import <React/RCTUtils.h>
#else
#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"
#endif

@import MobileCenterPush;
@import RNMobileCenterShared;

#import "RNPushUtils.h"
#import "RNPushDelegate.h"

@implementation RNPush

static id<RNPushDelegate> pushDelegate;

RCT_EXPORT_MODULE();

- (void)push:(MSPush *)push didReceivePushNotification:(MSPushNotification *)pushNotification {
    NSString *message = pushNotification.message;
}

- (instancetype)init
{
    self = [super init];
    
    if (self) {
        [pushDelegate setEventEmitter:self];
    }
    
    return self;
}

- (NSDictionary *)constantsToExport
{
    return @{};
}

- (NSArray<NSString *> *)supportedEvents
{
    return [pushDelegate supportedEvents];
}

+ (void)register
{
    pushDelegate = [[RNPushDelegateBase alloc] init];

    [RNMobileCenterShared configureMobileCenter];
    [MSPush setDelegate:pushDelegate];
    [MSMobileCenter startService:[MSPush class]];
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSPush isEnabled]]);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)shouldEnable
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSPush setEnabled:shouldEnable];
    resolve(nil);
}

RCT_EXPORT_METHOD(sendAndClearInitialNotification:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    [pushDelegate sendAndClearInitialNotification];
    resolve(nil);
}

@end
