#import "AppCenterReactNativePush.h"

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

#import <AppCenter/MSAppCenter.h>
#import <AppCenterPush/AppCenterPush.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

#import "AppCenterReactNativePushUtils.h"
#import "AppCenterReactNativePushDelegate.h"

@implementation AppCenterReactNativePush

static id<AppCenterReactNativePushDelegate> pushDelegate;

RCT_EXPORT_MODULE();

- (void)push:(MSPush *)push didReceivePushNotification:(MSPushNotification *)pushNotification {
}

- (instancetype)init
{
    self = [super init];
    
    if (self) {
        [pushDelegate setEventEmitter:self];
    }
    
    return self;
}

+ (BOOL)requiresMainQueueSetup
{
    return NO;
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
    pushDelegate = [[AppCenterReactNativePushDelegateBase alloc] init];

    [AppCenterReactNativeShared configureAppCenter];
    [MSPush setDelegate:pushDelegate];
    [MSAppCenter startService:[MSPush class]];
}

- (void)startObserving {
    // Will be called when this module's first listener is added.
    [pushDelegate startObserving];
}

- (void)stopObserving {
    // Will be called when this module's last listener is removed, or on dealloc.
    [pushDelegate stopObserving];
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
