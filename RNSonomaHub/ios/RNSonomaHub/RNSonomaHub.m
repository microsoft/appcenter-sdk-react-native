#import "RNSonomaHub.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import <AvalancheHub/AvalancheHub.h>

@interface RNSonomaHub () <RCTBridgeModule>
@end

@implementation RNSonomaHub

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(setEnabled:(BOOL)isEnabled
                    resolver:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    [AVAAvalanche setEnabled:isEnabled];
    resolve(nil);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                   rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[AVAAvalanche isEnabled]]);
}


RCT_EXPORT_METHOD(logLevel:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithInteger:[AVAAvalanche logLevel]]);
}

RCT_EXPORT_METHOD(setLogLevel:(AVALogLevel)logLevel
                     resolver:(RCTPromiseResolveBlock)resolve
                     rejecter:(RCTPromiseRejectBlock)reject)
{
    [AVAAvalanche setLogLevel:logLevel];
    resolve(nil);
}

RCT_EXPORT_METHOD(installId:(RCTPromiseResolveBlock)resolve
                   rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([[AVAAvalanche installId] UUIDString]);
}

- (NSDictionary *)constantsToExport
{
    return @{ @"AVALogLevelNone": @(AVALogLevelNone),
              @"AVALogLevelAssert": @(AVALogLevelAssert),
              @"AVALogLevelError": @(AVALogLevelError),
              @"AVALogLevelWarning": @(AVALogLevelWarning),
              @"AVALogLevelDebug": @(AVALogLevelDebug),
              @"AVALogLevelVerbose": @(AVALogLevelVerbose) };
};

@end
