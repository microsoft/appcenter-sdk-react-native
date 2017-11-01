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
#import "RNCrashesDelegate.h"

#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wincomplete-umbrella"
#import <MobileCenterCrashes/MSWrapperCrashesHelper.h>
#pragma GCC diagnostic pop

@import MobileCenterCrashes;
@import RNMobileCenterShared;

@implementation RNCrashes

static const int kMSUserConfirmationDontSendJS = 0;
static const int kMSUserConfirmationSendJS = 1;
static const int kMSUserConfirmationAlwaysSendJS = 2;

static dispatch_once_t onceToken;
static RNCrashesDelegate *crashesDelegate = nil;

RCT_EXPORT_MODULE();

+ (void)register
{
    [RNMobileCenterShared configureMobileCenter];
    [MSWrapperCrashesHelper setAutomaticProcessing:NO];
    [MSCrashes setDelegate:[RNCrashes sharedCrashesDelegate]];

    [MSMobileCenter startService:[MSCrashes class]];
}

+ (void)registerWithAutomaticProcessing
{
    [RNMobileCenterShared configureMobileCenter];
    [MSCrashes setDelegate:[RNCrashes sharedCrashesDelegate]];

    [MSMobileCenter startService:[MSCrashes class]];
}

- (instancetype)init
{
    self = [super init];

    if (self) {
        [crashesDelegate setEventEmitter:self];
    }

    return self;
}

- (NSDictionary *)constantsToExport
{
    return @{};
}

- (NSArray<NSString *> *)supportedEvents
{
    return [crashesDelegate supportedEvents];
}

RCT_EXPORT_METHOD(hasCrashedInLastSession:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    void (^fetchHasCrashedInLastSession)(void) = ^void() {
        MSErrorReport *report = [MSCrashes lastSessionCrashReport];
        resolve(report != nil ? @YES : @NO);
    };
    dispatch_async(dispatch_get_main_queue(), fetchHasCrashedInLastSession);
}

RCT_EXPORT_METHOD(lastSessionCrashReport:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject)
{
    void (^fetchLastSessionCrashReport)(void) = ^void() {
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
        case kMSUserConfirmationDontSendJS:
            [MSCrashes notifyWithUserConfirmation:MSUserConfirmationDontSend];
            break;
        case kMSUserConfirmationSendJS:
            [MSCrashes notifyWithUserConfirmation:MSUserConfirmationSend];
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
    NSArray *unprocessedCrashReports = [MSWrapperCrashesHelper unprocessedCrashReports];
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

+ (RNCrashesDelegate*) sharedCrashesDelegate {
    dispatch_once(&onceToken, ^{
        if (crashesDelegate == nil) {
            crashesDelegate = [RNCrashesDelegate new];
        }
    });
    return crashesDelegate;
}

@end
