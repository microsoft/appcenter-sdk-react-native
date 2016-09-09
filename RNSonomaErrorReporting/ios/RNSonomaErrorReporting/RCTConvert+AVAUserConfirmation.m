#import <AvalancheErrorReporting/AvalancheErrorReporting.h>
#import "RCTConvert.h"

@implementation RCTConvert (AVAUserConfirmation)

RCT_ENUM_CONVERTER(AVAUserConfirmation,
                   (@{ @"AVAUserConfirmationDontSend": @(AVAUserConfirmationDontSend),
                       @"AVAUserConfirmationSend": @(AVAUserConfirmationSend),
                       @"AVAUserConfirmationAlways": @(AVAUserConfirmationAlways) }),
                   AVAUserConfirmationDontSend,
                   integerValue)

@end
