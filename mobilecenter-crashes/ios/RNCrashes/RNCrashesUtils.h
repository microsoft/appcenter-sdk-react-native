#import <Foundation/Foundation.h>

@import MobileCenterCrashes.MSErrorReport;

NSDictionary* convertReportToJS(MSErrorReport* report);
NSArray* convertReportsToJS(NSArray* reports);
