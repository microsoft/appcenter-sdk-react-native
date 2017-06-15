#import "RNMobileCenterWrapper.h"

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

@import RNMobileCenter;

@interface RNMobileCenterWrapper () <RCTBridgeModule>
@end

@implementation RNMobileCenterWrapper

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(setCustomProperties:(NSDictionary*)properties
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [RNMobileCenter setCustomProperties:properties];
    resolve(nil);
}

@end
