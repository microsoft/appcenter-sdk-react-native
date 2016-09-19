#import "RNSonomaCore.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import <SonomaCore/SonomaCore.h>

@interface RNSonomaCore () <RCTBridgeModule>
@end

@implementation RNSonomaCore

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(setEnabled:(BOOL)isEnabled
                    resolver:(RCTPromiseResolveBlock)resolve
                    rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMSonoma setEnabled:isEnabled];
    resolve(nil);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                   rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[SNMSonoma isEnabled]]);
}


RCT_EXPORT_METHOD(getLogLevel:(RCTPromiseResolveBlock)resolve
                     rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithInteger:[SNMSonoma logLevel]]);
}

RCT_EXPORT_METHOD(setLogLevel:(SNMLogLevel)logLevel
                     resolver:(RCTPromiseResolveBlock)resolve
                     rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMSonoma setLogLevel:logLevel];
    resolve(nil);
}

RCT_EXPORT_METHOD(getInstallId:(RCTPromiseResolveBlock)resolve
                      rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([[SNMSonoma installId] UUIDString]);
}

- (NSDictionary *)constantsToExport
{
    return @{ @"LogLevelNone": @(SNMLogLevelNone),
              @"LogLevelAssert": @(SNMLogLevelAssert),
              @"LogLevelError": @(SNMLogLevelError),
              @"LogLevelWarning": @(SNMLogLevelWarning),
              @"LogLevelDebug": @(SNMLogLevelDebug),
              @"LogLevelVerbose": @(SNMLogLevelVerbose) };
};

@end
