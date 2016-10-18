#import <Foundation/Foundation.h>

#import <SonomaCrashes/SNMCrashesDelegate.h>
#import "RCTBridge.h"

@class RNSonomaCrashes;
@class SNMErrorReport;

@protocol RNSonomaCrashesDelegate <SNMCrashesDelegate>
// Call to expose a report to JS
- storeReportForJS:(SNMErrorReport *) report;

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

@interface RNSonomaCrashesDelegateBase : NSObject<RNSonomaCrashesDelegate>

@end

@interface RNSonomaCrashesDelegateAlwaysSend: RNSonomaCrashesDelegateBase

@end