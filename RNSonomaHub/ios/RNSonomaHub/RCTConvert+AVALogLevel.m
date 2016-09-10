#import <AvalancheHub/AvalancheHub.h>
#import "RCTConvert.h"

@implementation RCTConvert (AVALogLevel)

RCT_ENUM_CONVERTER(AVALogLevel,
                   (@{ @"AVALogLevelNone": @(AVALogLevelNone),
                       @"AVALogLevelAssert": @(AVALogLevelAssert),
                       @"AVALogLevelError": @(AVALogLevelError),
                       @"AVALogLevelWarning": @(AVALogLevelWarning),
                       @"AVALogLevelDebug": @(AVALogLevelDebug),
                       @"AVALogLevelVerbose": @(AVALogLevelVerbose) }),
                   AVALogLevelAssert,
                   integerValue)
@end
