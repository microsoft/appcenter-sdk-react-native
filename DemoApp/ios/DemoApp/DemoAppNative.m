#import "DemoAppNative.h"

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

@interface DemoAppNative () <RCTBridgeModule>
@end

@implementation DemoAppNative

static NSString* const kAppCenterSecretKey = @"AppSecret";
static NSString* const kAppCenterStartAutomaticallyKey = @"StartAutomatically";
static NSString* const kAppCenterConfigResource = @"AppCenter-Config";

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(configureStartup:(NSString*)secretString
                                  :(BOOL)startAutomatically)
{
  NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];
  if (secretString == nil) {
    secretString = @"";
  }
  [userDefaults setObject:secretString forKey:kAppCenterSecretKey];
  [userDefaults setBool:startAutomatically forKey:kAppCenterStartAutomaticallyKey];
}

RCT_EXPORT_METHOD(generateTestCrash)
{
  int* p = 0;
  *p = 0;
}

@end
