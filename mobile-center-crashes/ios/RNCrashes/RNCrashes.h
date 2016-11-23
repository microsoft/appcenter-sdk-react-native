#import <Foundation/Foundation.h>

#import "RNCrashesDelegate.h"

@interface RNCrashes : NSObject

+ (void)register;
+ (void)registerWithCrashDelegate:(id<RNCrashesDelegate>)delegate;

@end
