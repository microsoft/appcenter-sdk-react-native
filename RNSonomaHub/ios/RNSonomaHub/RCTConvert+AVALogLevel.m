#import <AvalancheHub/AvalancheHub.h>
#import "RCTConvert.h"

@implementation RCTConvert (AVALogLevel)

RCT_ENUM_CONVERTER(AVALogLevel,
                   (@{ @"LogLevelNone": @(AVALogLevelNone),
                       @"LogLevelAssert": @(AVALogLevelAssert),
                       @"LogLevelError": @(AVALogLevelError),
                       @"LogLevelWarning": @(AVALogLevelWarning),
                       @"LogLevelDebug": @(AVALogLevelDebug),
                       @"LogLevelVerbose": @(AVALogLevelVerbose) }),
                   AVALogLevelAssert,
                   integerValue)
@end
