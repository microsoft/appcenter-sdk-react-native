#import <Foundation/Foundation.h>

@import MobileCenterPush;

#if __has_include(<React/RCTBridge.h>)
#import <React/RCTBridge.h>
#else
#import "RCTBridge.h"
#endif

@class RNPush;

@protocol RNPushDelegate <MSPushDelegate>

@required
- (void) setBridge: (RCTBridge*) bridge;
@end

@interface RNPushDelegateBase : NSObject<RNPushDelegate>
@property RCTBridge* bridge;
@end
