#import "RNMobileCenter.h"

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

@import MobileCenter;
@import RNMobileCenterShared;

@interface RNMobileCenter () <RCTBridgeModule>
@end

@implementation RNMobileCenter

RCT_EXPORT_MODULE();

+ (void)register
{
    [RNMobileCenterShared configureMobileCenter];
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)enabled
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSMobileCenter setEnabled:enabled];
    resolve(nil);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSMobileCenter isEnabled]]);
}

RCT_EXPORT_METHOD(setLogLevel:(NSInteger)logLevel
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSMobileCenter setLogLevel:logLevel];
    resolve(nil);
}

RCT_EXPORT_METHOD(getLogLevel:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithInt:[MSMobileCenter logLevel]]);
}

RCT_EXPORT_METHOD(getInstallId:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([[MSMobileCenter installId] UUIDString]);
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
    [MSMobileCenter setCustomProperties:customProperties];
    resolve(nil);
}

@end
