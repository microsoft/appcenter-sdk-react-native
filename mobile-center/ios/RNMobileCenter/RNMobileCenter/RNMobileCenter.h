#import <Foundation/Foundation.h>

@import MobileCenter;

@interface RNMobileCenter : NSObject

+ (void) setEnabled:(BOOL) enabled;

+ (void) setLogLevel: (MSLogLevel)logLevel;
+ (MSLogLevel) logLevel;

+ (void) setCustomProperties: (NSDictionary*)properties;

@end
