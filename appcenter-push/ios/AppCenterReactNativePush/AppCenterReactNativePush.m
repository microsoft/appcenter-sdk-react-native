// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

static NSString *const kEnablePushInJavascript = @"EnablePushInJavascript";

static NSString *const kPushOnceEnabled = @"PushOnceEnabled";

static BOOL startedPush = NO;

static id<AppCenterReactNativePushDelegate> pushDelegate;

RCT_EXPORT_MODULE();

- (void)push:(MSPush *)push didReceivePushNotification:(MSPushNotification *)pushNotification {
}

- (instancetype)init {
  self = [super init];

  if (self) {
    [pushDelegate setEventEmitter:self];
  }

  return self;
}

+ (BOOL)requiresMainQueueSetup {
  return NO;
}

- (NSDictionary *)constantsToExport {
  return @{};
}

- (NSArray<NSString *> *)supportedEvents {
  return [pushDelegate supportedEvents];
}

+ (void)register {
  pushDelegate = [[AppCenterReactNativePushDelegateBase alloc] init];
  [MSPush setDelegate:pushDelegate];
  [AppCenterReactNativeShared configureAppCenter];
  if ([MSAppCenter isConfigured]) {
    BOOL startPush = YES;
    id enablePushInJavascript = [AppCenterReactNativeShared getConfiguration][kEnablePushInJavascript];
    if ([enablePushInJavascript isKindOfClass:[NSNumber class]] && [enablePushInJavascript boolValue]) {
      startPush = [[NSUserDefaults standardUserDefaults] boolForKey:kPushOnceEnabled];
    }
    if (startPush) {
      [MSAppCenter startService:[MSPush class]];
      startedPush = YES;
    }
  }
}

- (void)startObserving {
  // Will be called when this module's first listener is added.
  [pushDelegate startObserving];
}

- (void)stopObserving {
  // Will be called when this module's last listener is removed, or on dealloc.
  [pushDelegate stopObserving];
}

RCT_EXPORT_METHOD(isEnabled : (RCTPromiseResolveBlock)resolve rejecter : (RCTPromiseRejectBlock)reject) {
  resolve([NSNumber numberWithBool:[MSPush isEnabled]]);
}

RCT_EXPORT_METHOD(setEnabled : (BOOL)shouldEnable resolver : (RCTPromiseResolveBlock)resolve rejecter : (RCTPromiseRejectBlock)reject) {
  // [UIApplication registerForRemoteNotifications] should be called from the main thread.
  dispatch_async(dispatch_get_main_queue(), ^(void) {
    if (!startedPush && shouldEnable) {
      [MSAppCenter startService:[MSPush class]];
      startedPush = YES;
    }
    [MSPush setEnabled:shouldEnable];
    if (shouldEnable) {
      [[NSUserDefaults standardUserDefaults] setBool:YES forKey:kPushOnceEnabled];
    }
    resolve(nil);
  });
}

RCT_EXPORT_METHOD(sendAndClearInitialNotification : (RCTPromiseResolveBlock)resolve rejecter : (RCTPromiseRejectBlock)reject) {
  [pushDelegate sendAndClearInitialNotification];
  resolve(nil);
}

@end
