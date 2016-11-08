#import <Foundation/Foundation.h>

@import MobileCenter;

@interface RNMobileCenter : NSObject

+ (void) setAppSecret: (NSString *)secret;

+ (NSString *) getAppSecret;

+ (void) initializeMobileCenter;

+ (void) setEnabled:(BOOL) enabled;

+ (void) setLogLevel: (MSLogLevel)logLevel;
+ (MSLogLevel) logLevel;

+ (MSWrapperSdk *) getWrapperSdk;
+ (void) setWrapperSdk:(MSWrapperSdk *)sdk;

@end
