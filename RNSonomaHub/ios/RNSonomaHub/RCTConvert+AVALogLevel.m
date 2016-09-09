#import <AvalancheHub/AvalancheHub.h>
#import "RCTConvert.h"

@implementation RCTConvert (AVALogLevel)

RCT_ENUM_CONVERTER(AVALogLevel,
                   (@{ @"AVALogLevelNone": @(AVALogLevelNone),
                       @"AVALogLevelDebug": @(AVALogLevelError),
                       @"AVALogLevelWarning": @(AVALogLevelWarning),
                       @"AVALogLevelDebug": @(AVALogLevelDebug),
                       @"AVALogLevelVerbose": @(AVALogLevelVerbose) }),
                   AVALogLevelWarning,
                   integerValue)

@end
