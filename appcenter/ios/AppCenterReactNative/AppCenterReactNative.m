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
            [MSAppCenter startFromLibraryWithServices:@[serviceClass]];
        }
    }
    resolve(nil);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)enabled
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSAppCenter setEnabled:enabled];
    resolve(nil);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSAppCenter isEnabled]]);
}

RCT_EXPORT_METHOD(setLogLevel:(NSInteger)logLevel
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSAppCenter setLogLevel:logLevel];
    resolve(nil);
}

RCT_EXPORT_METHOD(getLogLevel:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithInt:[MSAppCenter logLevel]]);
}

RCT_EXPORT_METHOD(getInstallId:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([[MSAppCenter installId] UUIDString]);
}

RCT_EXPORT_METHOD(setUserId:(NSString *)userId
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSAppCenter setUserId:userId];
    resolve(nil);
}

RCT_EXPORT_METHOD(setCustomProperties:(NSDictionary*)properties
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    MSCustomProperties *customProperties = [MSCustomProperties new];
    for (NSString *key in properties) {
        id valueObject = [properties objectForKey:key];
        if (valueObject && [valueObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *valueDict = (NSDictionary *) valueObject;
            id type = [valueDict objectForKey:@"type"];
            id value = [valueDict objectForKey:@"value"];
            if (type && [type isKindOfClass:[NSString class]]) {
                NSString *typeString = (NSString *) type;
                if ([typeString isEqualToString:@"string"]) {
                    [customProperties setString:value forKey:key];
                }
                else if ([typeString isEqualToString:@"number"]) {
                    [customProperties setNumber:value forKey:key];
                }
                else if ([typeString isEqualToString:@"boolean"]) {
                    NSNumber *num = value;
                    [customProperties setBool:[num boolValue] forKey:key];
                }
                else if ([typeString isEqualToString:@"date-time"]) {
                    NSDate *date = [RCTConvert NSDate:value];
                    [customProperties setDate:date forKey:key];
                }
                else if ([typeString isEqualToString:@"clear"]) {
                    [customProperties clearPropertyForKey:key];
                }
            }
        }
    }
    [MSAppCenter setCustomProperties:customProperties];
    resolve(nil);
}

@end
