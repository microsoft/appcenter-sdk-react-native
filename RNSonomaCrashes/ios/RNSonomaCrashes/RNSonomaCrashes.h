#import <Foundation/Foundation.h>

#import "RNSonomaCrashesDelegate.h"

@interface RNSonomaCrashes : NSObject

+ (void)register;
+ (void)registerWithCrashDelegate:(id<RNSonomaCrashesDelegate>)delegate;

@end
