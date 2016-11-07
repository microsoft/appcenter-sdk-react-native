#import <Foundation/Foundation.h>

@import SonomaCore;

@interface RNMobileCenter : NSObject

+ (void) setAppSecret: (NSString *)secret;

+ (NSString *) getAppSecret;

+ (void) initializeMobileCenter;

+ (void) setEnabled:(BOOL) enabled;

+ (void) setLogLevel: (SNMLogLevel)logLevel;
+ (SNMLogLevel) logLevel;

+ (SNMWrapperSdk *) getWrapperSdk;
+ (void) setWrapperSdk:(SNMWrapperSdk *)sdk;

@end
