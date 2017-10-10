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
#import <MobileCenterCrashes/MSWrapperCrashesHelper.h>

@import MobileCenterCrashes;
@import RNMobileCenterShared;


@interface RNCrashes () <RCTBridgeModule>

@end

@implementation RNCrashes

static const int kMSUserConfirmationSendJS = 1;
static const int kMSUserConfirmationDontSendJS = 2;
static const int kMSUserConfirmationAlwaysSendJS = 3;

RCT_EXPORT_MODULE();

+ (void)register
{
    [RNMobileCenterShared configureMobileCenter];
    [MSWrapperCrashesHelper setAutomaticProcessing:NO];
    
    //[MSMobileCenter setLogLevel:MSLogLevelVerbose];     // Uncomment if needed for debugging

    [MSMobileCenter startService:[MSCrashes class]];
}

+ (void)registerWithAutomaticProcessing
{
    [RNMobileCenterShared configureMobileCenter];
    
    //[MSMobileCenter setLogLevel:MSLogLevelVerbose];     // Uncomment if needed for debugging

    [MSMobileCenter startService:[MSCrashes class]];
}

- (instancetype)init
{
    self = [super init];

    // Normally the bridge is nil at this point, but I left this code here anyway.
    // When the RNCrashes setBridge setter is called, below, is when the bridge is actually provided.
    if (self) {
    }

    return self;
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
        resolve(report != nil ? @YES : @NO);
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

RCT_EXPORT_METHOD(notifyWithUserConfirmation:(int)userConfirmation
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    switch (userConfirmation) {
        case kMSUserConfirmationSendJS:
            [MSCrashes notifyWithUserConfirmation:MSUserConfirmationSend];
            break;
        case kMSUserConfirmationDontSendJS:
            [MSCrashes notifyWithUserConfirmation:MSUserConfirmationDontSend];
            break;
        case kMSUserConfirmationAlwaysSendJS:
            [MSCrashes notifyWithUserConfirmation:MSUserConfirmationAlways];
            break;
        default:
            reject(@"notify_user_confirmation_failed", @"Invalid user confirmation value!", nil);
    }
    resolve(nil);
}

#pragma mark MSWrapperCrashesHelper Methods

/**
 * Gets a list of unprocessed crash reports.
 */
RCT_EXPORT_METHOD(getUnprocessedCrashReports:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
  void (^fetchUnprocessedCrashReports)() = ^void() {
    NSArray *unprocessedCrashReports = [MSWrapperCrashesHelper getUnprocessedCrashReports];
    resolve(convertReportsToJS(unprocessedCrashReports));
  };
  dispatch_async(dispatch_get_main_queue(), fetchUnprocessedCrashReports);
}

/**
 * Resumes processing for a list of error reports that is a subset of the unprocessed reports.
 */
RCT_EXPORT_METHOD(sendCrashReportsOrAwaitUserConfirmationForFilteredIds:(NSArray *)filteredIds
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
  BOOL alwaysSend = [MSWrapperCrashesHelper sendCrashReportsOrAwaitUserConfirmationForFilteredIds:filteredIds];
  resolve([NSNumber numberWithBool:alwaysSend]);
}

/**
 * Sends error attachments for a particular error report.
 */
RCT_EXPORT_METHOD(sendErrorAttachments:(NSArray *)errorAttachments
                  forIncidentIdentifier:(NSString *)incidentId
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
  [MSWrapperCrashesHelper sendErrorAttachments:convertJSAttachmentsToNativeAttachments(errorAttachments) withIncidentIdentifier:incidentId];
  resolve(nil);
}

@end
