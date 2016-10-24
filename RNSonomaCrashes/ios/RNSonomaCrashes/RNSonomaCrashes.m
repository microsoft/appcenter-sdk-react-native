#import "RNSonomaCrashes.h"

#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"

#import "RNSonomaCrashesUtils.h"

@import SonomaCrashes;

#import "RNSonomaCore.h" // TODO: revert back to <> syntax once it is built as a framework

//#import <SonomaCore/SonomaCore.h>

@interface RNSonomaCrashes () <RCTBridgeModule>

@end

@implementation RNSonomaCrashes

@synthesize bridge = _bridge;

static id<RNSonomaCrashesDelegate> crashDelegate;

RCT_EXPORT_MODULE();

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
        [crashDelegate setBridge:self.bridge];
    }

    return self;
}

- (NSDictionary *)constantsToExport
{
    SNMErrorReport *lastSessionCrashReport = [SNMCrashes lastSessionCrashReport];

    return @{
        @"hasCrashedInLastSession": @(lastSessionCrashReport != nil),
        @"lastCrashReport": convertReportToJS(lastSessionCrashReport)
    };
}

RCT_EXPORT_METHOD(getCrashReports:(RCTPromiseResolveBlock)resolve
		  rejecter:(RCTPromiseRejectBlock)reject)
{
  NSArray<SNMErrorReport *> *crashes = [crashDelegate getAndClearReports];
  resolve(convertReportsToJS(crashes));
}

RCT_EXPORT_METHOD(isDebuggerAttached:(RCTPromiseResolveBlock)resolve
                            rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[SNMSonoma isDebuggerAttached]]);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[SNMCrashes isEnabled]]);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)shouldEnable
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMCrashes setEnabled:shouldEnable];
    resolve(nil);
}

RCT_EXPORT_METHOD(generateTestCrash:(RCTPromiseResolveBlock)resolve
                           rejecter:(RCTPromiseRejectBlock)reject)
{
    [SNMCrashes generateTestCrash];
    reject(@"crash_failed", @"Failed to crash!", nil);
}

RCT_EXPORT_METHOD(crashUserResponse:(BOOL)send attachments:(NSDictionary *)attachments
                resolver:(RCTPromiseResolveBlock)resolve
                rejecter:(RCTPromiseRejectBlock)reject)
{
    SNMUserConfirmation response = send ? SNMUserConfirmationSend : SNMUserConfirmationDontSend;
    if ([crashDelegate respondsToSelector:@selector(reportUserResponse:)]) {
        [crashDelegate reportUserResponse:response];
    }
    [crashDelegate provideAttachments:attachments];
    [SNMCrashes notifyWithUserConfirmation:response];
    resolve(@"");
}

@end
