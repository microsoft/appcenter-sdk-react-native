#import "RNSonomaErrorReporting.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import <AvalancheErrorReporting/AvalancheErrorReporting.h>

@interface RNSonomaErrorReporting () <RCTBridgeModule>
@end

@implementation RNSonomaErrorReporting

RCT_EXPORT_MODULE();

- (NSDictionary *)constantsToExport
{
    return @{ @"AVAErrorLogSettingDisabled": @(AVAErrorLogSettingDisabled),
              @"AVAErrorLogSettingAlwaysAsk": @(AVAErrorLogSettingAlwaysAsk),
              @"AVAErrorLogSettingAutoSend": @(AVAErrorLogSettingAutoSend),
              @"AVAUserConfirmationDontSend": @(AVAUserConfirmationDontSend),
              @"AVAUserConfirmationSend": @(AVAUserConfirmationSend),
              @"AVAUserConfirmationAlways": @(AVAUserConfirmationAlways) };
};

RCT_EXPORT_METHOD(isDebuggerAttached:(RCTPromiseResolveBlock)resolve
                            rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[AVAErrorReporting isDebuggerAttached]]);
}

RCT_EXPORT_METHOD(generateTestCrash:(RCTPromiseResolveBlock)resolve
                           rejecter:(RCTPromiseRejectBlock)reject)
{
    [AVAErrorReporting generateTestCrash];
    resolve(nil);
}


RCT_EXPORT_METHOD(hasCrashedInLastSession:(RCTPromiseResolveBlock)resolve
                                 rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[AVAErrorReporting hasCrashedInLastSession]]);
}

@end
