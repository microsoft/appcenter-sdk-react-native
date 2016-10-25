#import <Foundation/Foundation.h>

@import SonomaCore;
//#import "/Users/jithomso/sonoma/react-native-sonoma-private/Sonoma-SDK-iOS/Products/SonomaCore/SonomaCore.framework/Headers/SonomaCore.h"
//#import <SonomaCore/SonomaCore.h>


@interface RNSonomaCore : NSObject

+ (void) setAppSecret: (NSString *)secret;

+ (NSString *) getAppSecret;

+ (void) initializeSonoma;

+ (void) setEnabled:(BOOL) enabled;

+ (void) setLogLevel: (SNMLogLevel)logLevel;
+ (SNMLogLevel) logLevel;

+ (SNMWrapperSdk *) getWrapperSdk;
+ (void) setWrapperSdk:(SNMWrapperSdk *)sdk;

@end
