#import "RNAnalytics.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

@import MobileCenterAnalytics;
@import RNMobileCenter;

@interface RNAnalytics () <RCTBridgeModule>
@end

@implementation RNAnalytics

RCT_EXPORT_MODULE();

+ (void)registerWithInitiallyEnabled:(BOOL) enabled
{
    [RNMobileCenter initializeMobileCenter];
    [MSAnalytics setEnabled:enabled];
    //[MSAnalytics setAutoPageTrackingEnabled:false]; // TODO: once the underlying SDK supports this, make sure to call this
    [MSMobileCenter startService:[MSAnalytics class]];
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
