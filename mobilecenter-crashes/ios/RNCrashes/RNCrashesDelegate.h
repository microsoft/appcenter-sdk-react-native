#import <Foundation/Foundation.h>

@import MobileCenterCrashes;

#import "RCTBridge.h"

@class RNCrashes;
@class MSErrorReport;

@protocol RNCrashesDelegate <MSCrashesDelegate>
// Call to expose a report to JS
- (void)storeReportForJS:(MSErrorReport *) report;

- (MSUserConfirmationHandler)shouldAwaitUserConfirmationHandler;


@optional
// Called when the JS code provides a send / don't-send response
- reportUserResponse: (MSUserConfirmation)confirmation;

@required
// Internal use only, to configure native <-> JS communication
- (NSArray<MSErrorReport *> *) getAndClearReports;
- (void) provideAttachments: (NSDictionary*) attachments;
- (void) setBridge: (RCTBridge*) bridge;
@end

@interface RNCrashesDelegateBase : NSObject<RNCrashesDelegate>
@property NSDictionary* attachments;
@property NSMutableArray* reports;
@property RCTBridge* bridge;
@end

@interface RNCrashesDelegateAlwaysSend: RNCrashesDelegateBase

@end
