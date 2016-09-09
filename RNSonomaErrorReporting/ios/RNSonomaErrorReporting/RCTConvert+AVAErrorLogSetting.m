#import <AvalancheErrorReporting/AvalancheErrorReporting.h>
#import "RCTConvert.h"

@implementation RCTConvert (AVAErrorLogSetting)

RCT_ENUM_CONVERTER(AVAErrorLogSetting,
                   (@{ @"AVAErrorLogSettingDisabled": @(AVAErrorLogSettingDisabled),
                       @"AVAErrorLogSettingAlwaysAsk": @(AVAErrorLogSettingAlwaysAsk),
                       @"AVAErrorLogSettingAutoSend": @(AVAErrorLogSettingAutoSend) }),
                   AVAErrorLogSettingDisabled,
                   integerValue)

@end
