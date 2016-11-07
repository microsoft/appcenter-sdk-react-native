#import <Foundation/Foundation.h>

@import Crashes.SNMErrorReport;

NSDictionary* convertReportToJS(SNMErrorReport* report);
NSArray* convertReportsToJS(NSArray* reports);
