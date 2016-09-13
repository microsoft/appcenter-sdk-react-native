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

- (instancetype)init
{
    self = [super init];

    if (self) {
        // TODO: Set custom userConfirmationHandler that bridges information to JS,
        // possibly via the RCTDeviceEventEmitter, so that a user can set a custom
        // handler and show a custom alert from JS.
        //
        // See [AVAErrorReporting setUserConfirmationHandler:userConfirmationHandler].
    }

    return self;
}

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

RCT_EXPORT_METHOD(sendCrashes:(RCTPromiseResolveBlock)resolve
                     rejecter:(RCTPromiseRejectBlock)reject)
{
    [AVAErrorReporting notifyWithUserConfirmation:AVAUserConfirmationSend];
    resolve(nil);
}

RCT_EXPORT_METHOD(ignoreCrashes:(RCTPromiseResolveBlock)resolve
                       rejecter:(RCTPromiseRejectBlock)reject)
{
    [AVAErrorReporting notifyWithUserConfirmation:AVAUserConfirmationDontSend];
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
    AVAErrorReport *lastSessionCrashDetails = [AVAErrorReporting lastSessionCrashDetails];
    // TODO: Serialize crash details to NSDictionary and send to JS.
    resolve(nil);
}

@end
