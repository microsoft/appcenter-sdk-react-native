#import "RNMobileCenter.h"

@implementation RNMobileCenter

+ (void) setEnabled:(BOOL) enabled
{
    [MSMobileCenter setEnabled:enabled];
}

+ (void) setLogLevel: (MSLogLevel)logLevel
{
    [MSMobileCenter setLogLevel:logLevel];
}

+ (MSLogLevel) logLevel
{
    return [MSMobileCenter logLevel];
}

+ (void) setCustomProperties:(NSDictionary*)properties {
    
    //TODO: for testing
    MSCustomProperties *customProperties = [MSCustomProperties new];
    [customProperties setString:@"blue" forKey:@"color"];
    [customProperties setNumber:@(10) forKey:@"score"];
    
    [MSMobileCenter setCustomProperties:customProperties];
}

@end
