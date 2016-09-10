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

RCT_EXPORT_METHOD(notifyWithUserConfirmation:(AVAUserConfirmation)userConfirmation
                                    resolver:(RCTPromiseResolveBlock)resolve
                                    rejecter:(RCTPromiseRejectBlock)reject)
{
    // After the error reporting prompt is shown to the user, this method is used
    // to communicate the user's reponse back from JS to native.
    [AVAErrorReporting notifyWithUserConfirmation:userConfirmation];
    resolve(nil);
}

RCT_EXPORT_METHOD(lastSessionCrashDetails:(RCTPromiseResolveBlock)resolve
                                 rejecter:(RCTPromiseRejectBlock)reject)
{
    AVAErrorReport *lastSessionCrashDetails = [AVAErrorReporting lastSessionCrashDetails];
    // TODO: Serialize crash details to NSDictionary and send to JS.
    resolve(nil);
}

@end
