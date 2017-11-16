#import "AppCenterReactNativeShared.h"

@implementation AppCenterReactNativeShared

static NSString *appSecret;
static MSWrapperSdk * wrapperSdk;

+ (void) setAppSecret: (NSString *)secret
{
  appSecret = secret;
}

+ (NSString *) getAppSecret
{
  if (appSecret == nil) {
    NSString * plistPath = [[NSBundle mainBundle] pathForResource:@"AppCenter-Config" ofType:@"plist"];
    NSDictionary * config = [NSDictionary dictionaryWithContentsOfFile:plistPath];

    appSecret = [config objectForKey:@"AppSecret"];
    // If the AppSecret is not set, we will pass nil to MSAppCenter which will error out, as expected
  }

  return appSecret;
}

+ (void) configureAppCenter
{
  if (![MSAppCenter isConfigured]) {
      MSWrapperSdk * wrapperSdk =
        [[MSWrapperSdk alloc]
            initWithWrapperSdkVersion:@"1.0.1"
            wrapperSdkName:@"appcenter.react-native"
            wrapperRuntimeVersion:nil
            liveUpdateReleaseLabel:nil
            liveUpdateDeploymentKey:nil
            liveUpdatePackageHash:nil];
      [self setWrapperSdk:wrapperSdk];
      [MSAppCenter configureWithAppSecret:[AppCenterReactNativeShared getAppSecret]];
  }
}

+ (MSWrapperSdk *) getWrapperSdk {
    return wrapperSdk;
}
+ (void) setWrapperSdk:(MSWrapperSdk *)sdk {
    wrapperSdk = sdk;
    [MSAppCenter setWrapperSdk:sdk];
}

@end
