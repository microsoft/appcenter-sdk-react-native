#import "RNSonomaAnalytics.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import <SonomaAnalytics/SonomaAnalytics.h>

@interface RNSonomaAnalytics () <RCTBridgeModule>
@end

@implementation RNSonomaAnalytics

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(trackEvent:(NSString *)eventName
              withProperties:(NSDictionary *)properties
                    resolver:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMAnalytics trackEvent:eventName withProperties:properties];
    resolve(nil);
}

RCT_EXPORT_METHOD(trackPage:(NSString *)pageName
             withProperties:(NSDictionary *)properties
                   resolver:(RCTPromiseResolveBlock)resolve
                   rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMAnalytics trackPage:pageName withProperties:properties];
    resolve(nil);
}

@end
