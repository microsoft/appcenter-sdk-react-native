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

@interface RNMobileCenter () <RCTBridgeModule>
@end

@implementation RNMobileCenter

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(setEnabled:(BOOL)enabled
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSMobileCenter setEnabled:enabled];
    resolve(nil);
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

RCT_EXPORT_METHOD(setCustomProperties:(NSDictionary*)properties
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    if (properties == nil) {
        resolve(nil);
        return;
    }

    MSCustomProperties *customProperties = [MSCustomProperties new];

    for (NSString *key in properties) {
        if ([[properties valueForKey:key] isKindOfClass:[NSString class]]) {
            [customProperties setString:[properties objectForKey:key] forKey:key];
        } else if ([[properties valueForKey:key] isKindOfClass:[NSNumber class]]) {
            [customProperties setNumber:[properties objectForKey:key] forKey:key];
        } else if ([[properties valueForKey:key] isKindOfClass:[NSDate class]]) {
            [customProperties setDate:[properties objectForKey:key] forKey:key];
        }
    }

    [MSMobileCenter setCustomProperties:customProperties];
    resolve(nil);
}

@end
