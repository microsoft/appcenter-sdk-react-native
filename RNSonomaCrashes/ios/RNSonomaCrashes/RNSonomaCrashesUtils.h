#import <Foundation/Foundation.h>

@import SonomaCrashes.SNMErrorReport;

NSDictionary* convertReportToJS(SNMErrorReport* report);
NSArray* convertReportsToJS(NSArray* reports);
