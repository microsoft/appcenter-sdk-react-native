#import "RNSonomaCrashes.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import "RNSonomaCrashesDelegate.h"
#import "RNSonomaCrashesUtils.h"

#import <SonomaCrashes/SonomaCrashes.h>
#import "RNSonomaCore.h"
#import <SonomaCore/SonomaCore.h>

@interface RNSonomaCrashes () <RCTBridgeModule>

@property id<RNSonomaCrashesDelegate> crashDelegate;

@end

@implementation RNSonomaCrashes

@synthesize bridge = _bridge;
@synthesize crashDelegate = _crashDelegate;

RCT_EXPORT_MODULE();

static id<RNSonomaCrashesDelegate> crashDelegate;

+ (void)register
{
    [RNSonomaCrashes registerWithCrashDelegate:[[RNSonomaCrashesDelegateBase alloc] init]];
}

+ (void)registerWithCrashDelegate:(id<RNSonomaCrashesDelegate>)delegate
{
  [RNSonomaCore initializeSonoma];
  [SNMCrashes setDelegate:delegate];
  crashDelegate = delegate;
  [SNMCrashes setUserConfirmationHandler:[delegate shouldAwaitUserConfirmationHandler]];
  [SNMSonoma startFeature:[SNMCrashes class]];
}

- (instancetype)init
{
    self = [super init];

    if (self) {
        [self.crashDelegate setBridge:self.bridge];
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

    NSArray<SNMErrorReport *> *crashes = [crashDelegate getAndClearReports];

    return @{
        @"hasCrashedInLastSession": @(lastSessionCrashReport == nil),
        @"lastCrashReport": convertReportToJS(lastSessionCrashReport),
        @"pendingErrors": convertReportsToJS(crashes)
    };
}

RCT_EXPORT_METHOD(isDebuggerAttached:(RCTPromiseResolveBlock)resolve
                            rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[SNMSonoma isDebuggerAttached]]);
}

RCT_EXPORT_METHOD(generateTestCrash:(RCTPromiseResolveBlock)resolve
                           rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMCrashes generateTestCrash];
    reject(nil);
}

RCT_EXPORT_METHOD(crashUserResponse:(BOOL)send attachments:(NSDictionary *)attachments
                resolver:(RCTPromiseResolveBlock)resolve
                rejecter:(RCTPromiseRejectBlock)reject)
{
    SNMUserConfirmation response = send ? SNMUserConfirmationSend : SNMUserConfirmationDontSend;
    if ([self.crashDelegate respondsToSelector:@selector(reportUserResponse:)]) {
        [self.crashDelegate reportUserResponse:response];
    }
    [self.crashDelegate provideAttachments:attachments];
    [SNMCrashes notifyWithUserConfirmation:response];
    resolve(@"");
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
    SNMErrorReport *lastSessionCrashDetails = [SNMCrashes lastSessionCrashReport];
    // TODO: Serialize crash details to NSDictionary and send to JS.
    resolve(nil);
}

@end
