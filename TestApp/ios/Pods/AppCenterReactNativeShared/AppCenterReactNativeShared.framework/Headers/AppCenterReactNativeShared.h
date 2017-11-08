#import <Foundation/Foundation.h>

@import AppCenter;

@interface AppCenterReactNativeShared : NSObject

+ (void) setAppSecret: (NSString *)secret;

+ (NSString *) getAppSecret;

+ (void) configureAppCenter;

+ (MSWrapperSdk *) getWrapperSdk;

+ (void) setWrapperSdk:(MSWrapperSdk *)sdk;

@end
