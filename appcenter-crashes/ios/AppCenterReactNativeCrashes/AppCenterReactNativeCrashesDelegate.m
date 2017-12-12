#import "AppCenterReactNativeCrashesDelegate.h"
#import "AppCenterReactNativeCrashesUtils.h"

static NSString *ON_BEFORE_SENDING_EVENT = @"AppCenterErrorReportOnBeforeSending";
static NSString *ON_SENDING_FAILED_EVENT = @"AppCenterErrorReportOnSendingFailed";
static NSString *ON_SENDING_SUCCEEDED_EVENT = @"AppCenterErrorReportOnSendingSucceeded";

@implementation AppCenterReactNativeCrashesDelegate

- (NSArray<NSString *> *)supportedEvents
{
    return @[ON_BEFORE_SENDING_EVENT, ON_SENDING_FAILED_EVENT, ON_SENDING_SUCCEEDED_EVENT];
}

- (void) crashes:(MSCrashes *)crashes willSendErrorReport:(MSErrorReport *)errorReport
{
    [self.eventEmitter sendEventWithName:ON_BEFORE_SENDING_EVENT body:convertReportToJS(errorReport)];
}

- (void) crashes:(MSCrashes *)crashes didSucceedSendingErrorReport:(MSErrorReport *)errorReport
{
    [self.eventEmitter sendEventWithName:ON_SENDING_SUCCEEDED_EVENT body:convertReportToJS(errorReport)];
}

- (void) crashes:(MSCrashes *)crashes didFailSendingErrorReport:(MSErrorReport *)errorReport withError:(NSError *)sendError
{
    [self.eventEmitter sendEventWithName:ON_SENDING_FAILED_EVENT body:convertReportToJS(errorReport)];
}

@end
