#import "RNCrashesDelegate.h"

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTEventDispatcher.h>)
#import <React/RCTEventDispatcher.h>
#else
#import "RCTEventDispatcher.h"
#endif

#import "RNCrashesUtils.h"

static NSString *ON_BEFORE_SENDING_EVENT = @"MobileCenterErrorReportOnBeforeSending";
static NSString *ON_SENDING_FAILED_EVENT = @"MobileCenterErrorReportOnSendingFailed";
static NSString *ON_SENDING_SUCCEEDED_EVENT = @"MobileCenterErrorReportOnSendingSucceeded";

@implementation RNCrashesDelegate

- (void) crashes:(MSCrashes *)crashes willSendErrorReport:(MSErrorReport *)errorReport
{
    [self.bridge.eventDispatcher sendAppEventWithName:ON_BEFORE_SENDING_EVENT body:convertReportToJS(errorReport)];
}

- (void) crashes:(MSCrashes *)crashes didSucceedSendingErrorReport:(MSErrorReport *)errorReport
{
    [self.bridge.eventDispatcher sendAppEventWithName:ON_SENDING_SUCCEEDED_EVENT body:convertReportToJS(errorReport)];
}

- (void) crashes:(MSCrashes *)crashes didFailSendingErrorReport:(MSErrorReport *)errorReport withError:(NSError *)sendError
{
    [self.bridge.eventDispatcher sendAppEventWithName:ON_SENDING_FAILED_EVENT body:convertReportToJS(errorReport)];
}

@end
