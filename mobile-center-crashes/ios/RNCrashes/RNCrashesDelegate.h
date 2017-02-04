#import <Foundation/Foundation.h>

@import MobileCenterCrashes;

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTBridge.h>)
#import <React/RCTBridge.h>
#else
#import "RCTBridge.h"
#endif

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
//TODO: Re-enable error attachment when the feature becomes available.
//- (void) provideAttachments: (NSDictionary*) attachments;
- (void) setBridge: (RCTBridge*) bridge;
@end

@interface RNCrashesDelegateBase : NSObject<RNCrashesDelegate>
//TODO: Re-enable error attachment when the feature becomes available.
//@property NSDictionary* attachments;
@property NSMutableArray* reports;
@property RCTBridge* bridge;
@end

@interface RNCrashesDelegateAlwaysSend: RNCrashesDelegateBase

@end
