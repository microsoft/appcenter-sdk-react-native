#import "RNCrashes.h"

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

#import "RNCrashesUtils.h"

@import MobileCenterCrashes;
@import RNMobileCenter;


@interface RNCrashes () <RCTBridgeModule>

@end

@implementation RNCrashes

@synthesize bridge = _bridge;

static id<RNCrashesDelegate> crashDelegate;

// iOS crash processing has a half second delay https://github.com/Microsoft/MobileCenter-SDK-iOS/blob/develop/MobileCenterCrashes/MobileCenterCrashes/MSCrashes.m#L296
static BOOL crashProcessingDelayFinished = NO;

RCT_EXPORT_MODULE();

+ (void)register
{
    [RNCrashes registerWithCrashDelegate:[[RNCrashesDelegateBase alloc] init]];
}

+ (void)registerWithCrashDelegate:(id<RNCrashesDelegate>)delegate
{
  [RNMobileCenter configureMobileCenter];
  [MSCrashes setDelegate:delegate];

  //[MSMobileCenter setLogLevel:MSLogLevelVerbose];     // Uncomment if needed for debugging

  crashDelegate = delegate;
  [MSCrashes setUserConfirmationHandler:[delegate shouldAwaitUserConfirmationHandler]];
  [MSMobileCenter startService:[MSCrashes class]];
  [self performSelector:@selector(crashProcessingDelayDidFinish) withObject:nil afterDelay:0.5];
}

+ (void)crashProcessingDelayDidFinish
{
    crashProcessingDelayFinished = YES;
}

- (instancetype)init
{
    self = [super init];

    // Normally the bridge is nil at this point, but I left this code here anyway.
    // When the RNCrashes setBridge setter is called, below, is when the bridge is actually provided.
    if (self) {
        [crashDelegate setBridge:self.bridge];
    }

    return self;
}

-(void)setBridge:(RCTBridge*) bridgeValue
{
    _bridge = bridgeValue;
    [crashDelegate setBridge:bridgeValue];
}

- (RCTBridge*) bridge {
    return _bridge;
}

- (NSDictionary *)constantsToExport
{
    return @{};
}

RCT_EXPORT_METHOD(hasCrashedInLastSession:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    void (^fetchHasCrashedInLastSession)() = ^void() {
        MSErrorReport *report = [MSCrashes lastSessionCrashReport];
        resolve(@(report != nil));
    };
    dispatch_async(dispatch_get_main_queue(), fetchHasCrashedInLastSession);
}

RCT_EXPORT_METHOD(lastSessionCrashReport:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    void (^fetchLastSessionCrashReport)() = ^void() {
        MSErrorReport *report = [MSCrashes lastSessionCrashReport];
        resolve(convertReportToJS(report));
    };
    dispatch_async(dispatch_get_main_queue(), fetchLastSessionCrashReport);
}

RCT_EXPORT_METHOD(getCrashReports:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    void (^fetchCrashReports)() = ^void() {
        resolve(convertReportsToJS([crashDelegate getAndClearReports]));
    };
    if (crashProcessingDelayFinished){
        fetchCrashReports();
    } else {
        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, NSEC_PER_SEC /2), dispatch_get_main_queue(), fetchCrashReports);
    }
}

RCT_EXPORT_METHOD(isDebuggerAttached:(RCTPromiseResolveBlock)resolve
                            rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSMobileCenter isDebuggerAttached]]);
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    resolve([NSNumber numberWithBool:[MSCrashes isEnabled]]);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)shouldEnable
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSCrashes setEnabled:shouldEnable];
    resolve(nil);
}

RCT_EXPORT_METHOD(generateTestCrash:(RCTPromiseResolveBlock)resolve
                           rejecter:(RCTPromiseRejectBlock)reject)
{
    [MSCrashes generateTestCrash];
    reject(@"crash_failed", @"Failed to crash!", nil);
}

RCT_EXPORT_METHOD(crashUserResponse:(BOOL)send attachments:(NSDictionary *)attachments
                resolver:(RCTPromiseResolveBlock)resolve
                rejecter:(RCTPromiseRejectBlock)reject)
{
    MSUserConfirmation response = send ? MSUserConfirmationSend : MSUserConfirmationDontSend;
    if ([crashDelegate respondsToSelector:@selector(reportUserResponse:)]) {
        [crashDelegate reportUserResponse:response];
    }
    //TODO: Re-enable error attachment when the feature becomes available.
    //[crashDelegate provideAttachments:attachments];
    [MSCrashes notifyWithUserConfirmation:response];
    resolve(@"");
}

@end
