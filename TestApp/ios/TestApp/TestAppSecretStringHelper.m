#import "TestAppSecretStringHelper.h"

#if __has_include(<React/RCTAssert.h>)
#import <React/RCTAssert.h>
#import <React/RCTBridgeModule.h>
#import <React/RCTConvert.h>
#import <React/RCTEventDispatcher.h>
#import <React/RCTRootView.h>
#import <React/RCTUtils.h>
#else
#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"
#endif

#import <AppCenter/AppCenter.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface TestAppSecretStringHelper () <RCTBridgeModule>
@end

@implementation TestAppSecretStringHelper

static const NSString* kAppCenterSecretKey = @"AppSecret";
static const NSString* kAppCenterConfigResource = @"AppCenter-Config";

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(configureStartup:(NSString*)secretString)
{
  NSString *plistPath = [[NSBundle mainBundle] pathForResource:kAppCenterConfigResource ofType:@"plist"];
  NSDictionary *config = [NSMutableDictionary dictionaryWithContentsOfFile:plistPath];
  [config setValue:secretString forKey:kAppCenterSecretKey];
  [config writeToFile:plistPath atomically:YES];
}

@end
