// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNativeCrashes.h"

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

#import "AppCenterReactNativeCrashesUtils.h"
#import "AppCenterReactNativeCrashesDelegate.h"

#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wincomplete-umbrella"
#import <AppCenterCrashes/MSWrapperCrashesHelper.h>
#pragma GCC diagnostic pop

#import <AppCenter/MSAppCenter.h>
#import <AppCenterCrashes/AppCenterCrashes.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@implementation AppCenterReactNativeCrashes

static const int kMSUserConfirmationDontSendJS = 0;
static const int kMSUserConfirmationSendJS = 1;
static const int kMSUserConfirmationAlwaysSendJS = 2;

static dispatch_once_t onceToken;
static AppCenterReactNativeCrashesDelegate *crashesDelegate = nil;

RCT_EXPORT_MODULE();

+ (void)register
{
    [MSWrapperCrashesHelper setAutomaticProcessing:NO];
    [MSCrashes setDelegate:[AppCenterReactNativeCrashes sharedCrashesDelegate]];
    [AppCenterReactNativeShared configureAppCenter];
    if ([MSAppCenter isConfigured]) {
      [MSAppCenter startService:[MSCrashes class]];
    }
}

+ (void)registerWithAutomaticProcessing
{
    [AppCenterReactNativeShared configureAppCenter];
    [MSCrashes setDelegate:[AppCenterReactNativeCrashes sharedCrashesDelegate]];

    [MSAppCenter startService:[MSCrashes class]];
}

- (instancetype)init
{
    self = [super init];

    if (self) {
        [crashesDelegate setEventEmitter:self];
    }

    return self;
}

+ (BOOL)requiresMainQueueSetup
{
    return NO;
}

- (NSDictionary *)constantsToExport
{
    return @{};
}

- (NSArray<NSString *> *)supportedEvents
{
    return [crashesDelegate supportedEvents];
}

- (void)startObserving {
    // Will be called when this module's first listener is added.
    [crashesDelegate startObserving];
}

- (void)stopObserving {
    // Will be called when this module's last listener is removed, or on dealloc.
    [crashesDelegate stopObserving];
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
    resolve([NSNumber numberWithBool:[MSAppCenter isDebuggerAttached]]);
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
    resolve(nil);
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
            // Let native SDK valitade and log an error for unknown value
            [MSCrashes notifyWithUserConfirmation:userConfirmation];
            break;
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
  void (^fetchUnprocessedCrashReports)(void) = ^void() {
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

+ (AppCenterReactNativeCrashesDelegate*) sharedCrashesDelegate {
    dispatch_once(&onceToken, ^{
        if (crashesDelegate == nil) {
            crashesDelegate = [AppCenterReactNativeCrashesDelegate new];
        }
    });
    return crashesDelegate;
}

@end
