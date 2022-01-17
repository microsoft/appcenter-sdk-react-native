// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNative.h"

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

#import <AppCenter/AppCenter.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface AppCenterReactNative () <RCTBridgeModule>
@end

@implementation AppCenterReactNative

RCT_EXPORT_MODULE();

+ (void)register
{
    [AppCenterReactNativeShared configureAppCenter];
}

RCT_EXPORT_METHOD(startFromLibrary:(NSDictionary*)service
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    id bindingType = [service objectForKey:@"bindingType"];
    if ([bindingType isKindOfClass:[NSString class]]) {
        id serviceClass = NSClassFromString(bindingType);
        if (serviceClass) {
            [MSACAppCenter startFromLibraryWithServices:@[serviceClass]];
        }
    }
    resolve(nil);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)enabled
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSACAppCenter setEnabled:enabled];
    resolve(nil);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSACAppCenter isEnabled]]);
}

RCT_EXPORT_METHOD(setLogLevel:(NSInteger)logLevel
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSACAppCenter setLogLevel:logLevel];
    resolve(nil);
}

RCT_EXPORT_METHOD(getLogLevel:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithInt:[MSACAppCenter logLevel]]);
}

RCT_EXPORT_METHOD(getInstallId:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([[MSACAppCenter installId] UUIDString]);
}

RCT_EXPORT_METHOD(setUserId:(NSString *)userId
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSACAppCenter setUserId:userId];
    resolve(nil);
}

RCT_EXPORT_METHOD(setNetworkRequestsAllowed:(BOOL)isAllowed
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSACAppCenter setNetworkRequestsAllowed:isAllowed];
    resolve(nil);
}

RCT_EXPORT_METHOD(isNetworkRequestsAllowed:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSACAppCenter isNetworkRequestsAllowed]]);
}

@end
