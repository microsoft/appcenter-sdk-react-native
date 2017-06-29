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
    if (properties == nil) {
        resolve(nil);
        return;
    }

    MSCustomProperties *customProperties = [MSCustomProperties new];

    // TODO: Normally dates are fine here but we noticed weird behavior where on an iPhone 6 simulator, running debug, iOS 10.3
    // has dates end up as "string" type not "date_time". Max and Bret tried various combinations and iPhone 6 simulator running
    // non-debug, or iPhone 7 simulator on iOS 10.3 (debug or non-debug), or iPhone 6 simulator debug on max's machine (running iOS 9.x)
    // all worked. But that one combination of iPhone 6 simulator, debug, iOS 10.3 (on Bret's machine) consistently saw the
    // date properties end up as string type not "date_time" when sent to server. There's an issue there somewhere, which we
    // should ideally identify, but ignoring for now.
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
