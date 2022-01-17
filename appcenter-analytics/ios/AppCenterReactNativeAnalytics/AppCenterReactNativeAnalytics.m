// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNativeAnalytics.h"

// Support React Native headers both in the React namespace, where they are in
// RN version 0.40+, and no namespace, for older versions of React Native
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

#import <AppCenter/MSACAppCenter.h>
#import <AppCenterAnalytics/MSACAnalytics.h>
#import <AppCenterAnalytics/MSACAnalyticsTransmissionTarget.h>
#import <AppCenterAnalytics/MSACPropertyConfigurator.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

typedef NSMutableDictionary<NSString *, MSACAnalyticsTransmissionTarget *> MSACTargetsDictionary;

@interface AppCenterReactNativeAnalytics () <RCTBridgeModule>

@property(nonatomic) MSACTargetsDictionary *transmissionTargets;

@end

@implementation AppCenterReactNativeAnalytics

RCT_EXPORT_MODULE();

+ (void)registerWithInitiallyEnabled:(BOOL)enabled {
  [AppCenterReactNativeShared configureAppCenter];
  if ([MSACAppCenter isConfigured]) {
    [MSACAppCenter startService:[MSACAnalytics class]];
    if (!enabled) {
      [MSACAnalytics setEnabled:enabled];
    }
  }
}

- (MSACTargetsDictionary *)targetsForTokens {
  if (self.transmissionTargets == nil) {
    self.transmissionTargets = [MSACTargetsDictionary new];
  }
  return self.transmissionTargets;
}

RCT_EXPORT_METHOD(isEnabled : (RCTPromiseResolveBlock)resolve rejecter : (RCTPromiseRejectBlock)reject) {
  resolve(@([MSACAnalytics isEnabled]));
}

RCT_EXPORT_METHOD(setEnabled : (BOOL)shouldEnable resolver : (RCTPromiseResolveBlock)resolve rejecter : (RCTPromiseRejectBlock)reject) {
  [MSACAnalytics setEnabled:shouldEnable];
  resolve(nil);
}

RCT_EXPORT_METHOD(startSession : (RCTPromiseResolveBlock) resolve rejecter : (RCTPromiseRejectBlock)reject) {
  [MSACAnalytics startSession];
  resolve(nil);
}

RCT_EXPORT_METHOD(trackEvent
                  : (NSString *)eventName withProperties
                  : (NSDictionary *)properties resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  NSArray *allowedKeys = [[properties keysOfEntriesPassingTest:^BOOL(NSString *key, id obj, BOOL *stop) {
    if ([obj isKindOfClass:[NSDictionary class]] || [obj isKindOfClass:[NSArray class]]) {
      return NO;
    }
    return YES;
  }] allObjects];
  NSArray *newValues = [properties objectsForKeys:allowedKeys notFoundMarker:@""];
  NSDictionary *filteredProperties = [NSDictionary dictionaryWithObjects:newValues forKeys:allowedKeys];
  [MSACAnalytics trackEvent:eventName withProperties:filteredProperties];
  resolve(nil);
}

RCT_EXPORT_METHOD(trackTransmissionTargetEvent
                  : (NSString *)eventName properties
                  : (NSDictionary *)properties forTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  NSArray *allowedKeys = [[properties keysOfEntriesPassingTest:^BOOL(NSString *key, id obj, BOOL *stop) {
    if ([obj isKindOfClass:[NSDictionary class]] || [obj isKindOfClass:[NSArray class]]) {
      return NO;
    }
    return YES;
  }] allObjects];
  NSArray *newValues = [properties objectsForKeys:allowedKeys notFoundMarker:@""];
  NSDictionary *filteredProperties = [NSDictionary dictionaryWithObjects:newValues forKeys:allowedKeys];
  [transmissionTarget trackEvent:eventName withProperties:filteredProperties];
  resolve(nil);
}

RCT_EXPORT_METHOD(getTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [MSACAnalytics transmissionTargetForToken:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [[self targetsForTokens] setObject:transmissionTarget forKey:targetToken];
  resolve(targetToken);
}

RCT_EXPORT_METHOD(isTransmissionTargetEnabled
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  resolve([NSNumber numberWithBool:[transmissionTarget isEnabled]]);
}

RCT_EXPORT_METHOD(setTransmissionTargetEnabled
                  : (BOOL)shouldEnable targetToken
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget setEnabled:shouldEnable];
  resolve(nil);
}

RCT_EXPORT_METHOD(setTransmissionTargetEventProperty
                  : (NSString *)propertyKey propertyValue
                  : (NSString *)propertyValue forTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget.propertyConfigurator setEventPropertyString:propertyValue forKey:propertyKey];
  resolve(nil);
}

RCT_EXPORT_METHOD(removeTransmissionTargetEventProperty
                  : (NSString *)propertyKey forTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget.propertyConfigurator removeEventPropertyForKey:propertyKey];
  resolve(nil);
}

RCT_EXPORT_METHOD(collectTransmissionTargetDeviceId
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget.propertyConfigurator collectDeviceId];
  resolve(nil);
}

RCT_EXPORT_METHOD(getChildTransmissionTarget
                  : (NSString *)childToken forTransmissionTarget
                  : (NSString *)parentToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (parentToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:parentToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *childTarget = [transmissionTarget transmissionTargetForToken:childToken];
  if (childTarget == nil) {
    resolve(nil);
    return;
  }
  [[self targetsForTokens] setObject:childTarget forKey:childToken];
  resolve(childToken);
}

RCT_EXPORT_METHOD(setTransmissionTargetAppName
                  : (NSString *)appName forTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget.propertyConfigurator setAppName:appName];
  resolve(nil);
}

RCT_EXPORT_METHOD(setTransmissionTargetAppVersion
                  : (NSString *)appVersion forTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget.propertyConfigurator setAppVersion:appVersion];
  resolve(nil);
}

RCT_EXPORT_METHOD(setTransmissionTargetAppLocale
                  : (NSString *)appLocale forTransmissionTarget
                  : (NSString *)targetToken resolver
                  : (RCTPromiseResolveBlock)resolve rejecter
                  : (RCTPromiseRejectBlock)reject) {
  if (targetToken == nil) {
    resolve(nil);
    return;
  }
  MSACAnalyticsTransmissionTarget *transmissionTarget = [[self targetsForTokens] objectForKey:targetToken];
  if (transmissionTarget == nil) {
    resolve(nil);
    return;
  }
  [transmissionTarget.propertyConfigurator setAppLocale:appLocale];
  resolve(nil);
}

@end
