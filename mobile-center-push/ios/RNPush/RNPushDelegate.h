#import <Foundation/Foundation.h>

@import MobileCenterPush;

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTEventEmitter.h>)
#import <React/RCTEventEmitter.h>
#else
#import "RCTEventEmitter.h"
#endif

@class RNPush;

@protocol RNPushDelegate <MSPushDelegate>

@required
- (void) setEventEmitter: (RCTEventEmitter*) eventEmitter;

@required
- (void) sendAndClearInitialNotification;

@required
- (NSArray<NSString *> *)supportedEvents;

@end

@interface RNPushDelegateBase : NSObject<RNPushDelegate>
@property RCTEventEmitter* eventEmitter;
@property BOOL saveInitialNotification;
@property NSDictionary* initialNotification;
@end
