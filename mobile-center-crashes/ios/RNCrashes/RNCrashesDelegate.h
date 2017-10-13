#import <Foundation/Foundation.h>

@import MobileCenterCrashes;

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTBridge.h>)
#import <React/RCTBridge.h>
#else
#import "RCTBridge.h"
#endif


@interface RNCrashesDelegate : NSObject<MSCrashesDelegate>

@property RCTBridge* bridge;

@end

