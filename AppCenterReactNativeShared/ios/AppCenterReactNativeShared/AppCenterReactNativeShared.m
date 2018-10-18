#import "AppCenterReactNativeShared.h"
#import <AppCenter/MSAppCenter.h>
#import <AppCenter/MSWrapperSdk.h>

@implementation AppCenterReactNativeShared

static NSString* const kAppCenterSecretKey = @"AppSecret";
static NSString* const kAppCenterStartAutomaticallyKey = @"StartAutomatically";
static NSString* const kAppCenterConfigResource = @"AppCenter-Config";

static NSString *appSecret;
static BOOL startAutomatically;
static MSWrapperSdk *wrapperSdk;

+ (void) setAppSecret: (NSString *)secret
{
  appSecret = secret;
}

+ (NSString *) getAppSecret
{
  if (appSecret == nil) {
    NSString * plistPath = [[NSBundle mainBundle] pathForResource:kAppCenterConfigResource ofType:@"plist"];
    NSDictionary * config = [NSDictionary dictionaryWithContentsOfFile:plistPath];
    appSecret = [config objectForKey:kAppCenterSecretKey];

    // Read start automatically flag, by default it's true if not set.
    id rawStartAutomatically = [config objectForKey:kAppCenterStartAutomaticallyKey];
    if ([rawStartAutomatically isKindOfClass:[NSNumber class]]) {
      startAutomatically = [rawStartAutomatically boolValue];
    }
    else {
      startAutomatically = YES;
    }
  }
  return appSecret;
}

+ (void) configureAppCenter
{
  if (!wrapperSdk) {
      MSWrapperSdk * wrapperSdk =
        [[MSWrapperSdk alloc]
            initWithWrapperSdkVersion:@"1.9.0"
            wrapperSdkName:@"appcenter.react-native"
            wrapperRuntimeVersion:nil
            liveUpdateReleaseLabel:nil
            liveUpdateDeploymentKey:nil
            liveUpdatePackageHash:nil];
      [self setWrapperSdk:wrapperSdk];
      [AppCenterReactNativeShared getAppSecret];
      if (startAutomatically) {
        if ([appSecret length] == 0) {
          [MSAppCenter configure];
        } else {
          [MSAppCenter configureWithAppSecret:appSecret];
        }
      }
  }
}

+ (MSWrapperSdk *) getWrapperSdk {
    return wrapperSdk;
}

+ (void) setWrapperSdk:(MSWrapperSdk *)sdk {
    wrapperSdk = sdk;
    [MSAppCenter setWrapperSdk:sdk];
}

+ (void) setStartAutomatically:(BOOL)shouldStartAutomatically {
    startAutomatically = shouldStartAutomatically;
}

@end
