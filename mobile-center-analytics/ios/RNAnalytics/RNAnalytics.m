#import "RNAnalytics.h"

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

@import MobileCenterAnalytics;
@import RNMobileCenter;

@interface RNAnalytics () <RCTBridgeModule>
@end

@implementation RNAnalytics

RCT_EXPORT_MODULE();

+ (void)registerWithInitiallyEnabled:(BOOL) enabled
{
    [RNMobileCenter configureMobileCenter];
    if (!enabled) {
        // Avoid starting an analytics session.
        // Note that we don't call this if startEnabled is true, because
        // that causes a session to try and start before MSAnalytics is started.
        [MSAnalytics setEnabled:enabled];
    }
    //[MSAnalytics setAutoPageTrackingEnabled:false]; // TODO: once the underlying SDK supports this, make sure to call this
    [MSMobileCenter startService:[MSAnalytics class]];
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSAnalytics isEnabled]]);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)shouldEnable
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSAnalytics setEnabled:shouldEnable];
    resolve(nil);
}

RCT_EXPORT_METHOD(trackEvent:(NSString *)eventName
              withProperties:(NSDictionary *)properties
                    resolver:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    NSArray * allowedKeys = [[properties keysOfEntriesPassingTest:^BOOL (NSString *key, id obj, BOOL * stop) {
        if ([obj isKindOfClass:[NSDictionary class]] ||
            [obj isKindOfClass:[NSArray class]]) {
            return NO;
        }
        return YES;
    }] allObjects];
    NSArray * newValues = [properties objectsForKeys:allowedKeys notFoundMarker:@""];
    NSDictionary * filteredProperties = [NSDictionary dictionaryWithObjects:newValues forKeys:allowedKeys];
    [MSAnalytics trackEvent:eventName withProperties:filteredProperties];
    resolve(nil);
}

/*
// TODO: once the underlying SDK supports this
RCT_EXPORT_METHOD(trackPage:(NSString *)pageName
             withProperties:(NSDictionary *)properties
                   resolver:(RCTPromiseResolveBlock)resolve
                   rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSAnalytics trackPage:pageName withProperties:properties];
    resolve(nil);
}
*/


@end
