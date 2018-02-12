#import "AppCenterReactNativeAnalytics.h"

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
#import <AppCenterAnalytics/MSAnalytics.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface AppCenterReactNativeAnalytics () <RCTBridgeModule>
@end

@implementation AppCenterReactNativeAnalytics

RCT_EXPORT_MODULE();

+ (void)registerWithInitiallyEnabled:(BOOL) enabled
{
    [AppCenterReactNativeShared configureAppCenter];
    [MSAppCenter startService:[MSAnalytics class]];
    if (!enabled) {
        [MSAnalytics setEnabled:enabled];
    }
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

@end
