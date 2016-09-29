#import "RNSonomaCrashes.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import <SonomaCrashes/SonomaCrashes.h>
#import <RNSonomaCore/RNSonomaCore.h>
#import <SonomaCore/SonomaCore.h>

@interface RNSonomaCrashes () <RCTBridgeModule>
@end

@implementation RNSonomaCrashes

RCT_EXPORT_MODULE();

+ (void)register
{
  [RNSonomaCore initializeSonoma];
  [SNMSonoma startFeature:[SNMCrashes class]]
}

// TODO: support this once the underlying iOS supports it.
/*
+ (void)registerWithCrashDelegate
{

}
*/


- (instancetype)init
{
    self = [super init];

    if (self) {
        // TODO: Set custom userConfirmationHandler that bridges information to JS,
        // possibly via the RCTDeviceEventEmitter, so that a user can set a custom
        // handler and show a custom alert from JS.
        //
        // See [SNMCrashes setUserConfirmationHandler:userConfirmationHandler].
    }

    return self;
}

- (NSDictionary *)constantsToExport
{
    SNMErrorReport *lastSessionCrashReport = [SNMCrashes lastSessionCrashReport];

    // TODO: Serialize lastSessionCrashReport in a similar way to android
    return @{
        @"hasCrashedInLastSession": lastSessionCrashReport == nil
    };
}

RCT_EXPORT_METHOD(isDebuggerAttached:(RCTPromiseResolveBlock)resolve
                            rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[SNMCrashes isDebuggerAttached]]);
}

RCT_EXPORT_METHOD(generateTestCrash:(RCTPromiseResolveBlock)resolve
                           rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMCrashes generateTestCrash];
    reject(nil);
}


RCT_EXPORT_METHOD(hasCrashedInLastSession:(RCTPromiseResolveBlock)resolve
                                 rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[SNMCrashes hasCrashedInLastSession]]);
}

RCT_EXPORT_METHOD(sendCrashes:(RCTPromiseResolveBlock)resolve
                     rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMCrashes notifyWithUserConfirmation:SNMUserConfirmationSend];
    resolve(nil);
}

RCT_EXPORT_METHOD(ignoreCrashes:(RCTPromiseResolveBlock)resolve
                       rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMCrashes notifyWithUserConfirmation:SNMUserConfirmationDontSend];
    resolve(nil);
}

RCT_EXPORT_METHOD(setTextAttachment:(NSString *)textAttachment
                           resolver:(RCTPromiseResolveBlock)resolve
                           rejecter:(RCTPromiseRejectBlock)reject)
{
    // TODO
    resolve(nil);
}

RCT_EXPORT_METHOD(getLastSessionCrashDetails:(RCTPromiseResolveBlock)resolve
                                    rejecter:(RCTPromiseRejectBlock)reject)
{
    SNMErrorReport *lastSessionCrashDetails = [SNMCrashes lastSessionCrashDetails];
    // TODO: Serialize crash details to NSDictionary and send to JS.
    resolve(nil);
}

@end
