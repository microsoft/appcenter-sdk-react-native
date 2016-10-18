#import <Foundation/Foundation.h>
#import <SonomaCrashes/SonomaCrashes.h>

NSDictionary* convertReportToJS(SNMErrorReport* report);
NSArray* convertReportsToJS(NSArray* reports);
