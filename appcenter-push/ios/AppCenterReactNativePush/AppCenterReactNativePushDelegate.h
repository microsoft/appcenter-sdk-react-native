#import <Foundation/Foundation.h>

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTEventEmitter.h>)
#import <React/RCTEventEmitter.h>
#else
#import "RCTEventEmitter.h"
#endif

@class AppCenterReactNativePush;

@protocol MSPushDelegate;

@protocol AppCenterReactNativePushDelegate <MSPushDelegate>

@required
- (void)setEventEmitter:(RCTEventEmitter*)eventEmitter;

@required
- (void)sendAndClearInitialNotification;

@required
- (void)startObserving;

@required
- (void)stopObserving;

@required
- (NSArray<NSString *> *)supportedEvents;

@end

@interface AppCenterReactNativePushDelegateBase : NSObject<AppCenterReactNativePushDelegate>
@property RCTEventEmitter* eventEmitter;
@property BOOL saveInitialNotification;
@property NSDictionary* initialNotification;
@end
