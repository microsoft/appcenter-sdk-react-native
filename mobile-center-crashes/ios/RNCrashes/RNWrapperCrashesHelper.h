#import <Foundation/Foundation.h>

@class MSWrapperCrashesHelper;

@interface RNWrapperCrashesHelper : NSObject

/**
 * Disables automatic crash processing. Causes SDK not to send reports, even if ALWAYS_SEND is set.
 */
+ (void)setAutomaticProcessing:(BOOL)automaticProcessing;

@end

