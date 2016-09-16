#import <SonomaCore/SonomaCore.h>
#import "RCTConvert.h"

@implementation RCTConvert (SNMLogLevel)

RCT_ENUM_CONVERTER(SNMLogLevel,
                   (@{ @"LogLevelNone": @(SNMLogLevelNone),
                       @"LogLevelAssert": @(SNMLogLevelAssert),
                       @"LogLevelError": @(SNMLogLevelError),
                       @"LogLevelWarning": @(SNMLogLevelWarning),
                       @"LogLevelDebug": @(SNMLogLevelDebug),
                       @"LogLevelVerbose": @(SNMLogLevelVerbose) }),
                   SNMLogLevelAssert,
                   integerValue)
@end
