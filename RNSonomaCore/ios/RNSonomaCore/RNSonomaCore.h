#import <Foundation/Foundation.h>

#import <SonomaCore/SNMConstants.h>

@interface RNSonomaCore : NSObject

+ (void) setAppSecret: (NSString *)secret;

+ (NSString *) getAppSecret;

+ (void) initializeSonoma;

+ (void) setEnabled:(BOOL) enabled;

+ (void) setLogLevel: (SNMLogLevel)logLevel;
+ (SNMLogLevel) logLevel;

@end
