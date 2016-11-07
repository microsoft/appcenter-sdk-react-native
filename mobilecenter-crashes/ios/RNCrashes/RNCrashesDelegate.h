#import <Foundation/Foundation.h>

@import Crashes;

#import "RCTBridge.h"

@class RNCrashes;
@class SNMErrorReport;

@protocol RNCrashesDelegate <SNMCrashesDelegate>
// Call to expose a report to JS
- (void)storeReportForJS:(SNMErrorReport *) report;

- (SNMUserConfirmationHandler)shouldAwaitUserConfirmationHandler;


@optional
// Called when the JS code provides a send / don't-send response
- reportUserResponse: (SNMUserConfirmation)confirmation;

@required
// Internal use only, to configure native <-> JS communication
- (NSArray<SNMErrorReport *> *) getAndClearReports;
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
