#import "RNMobileCenterShared.h"

@implementation RNMobileCenterShared

static NSString *appSecret;
static MSWrapperSdk * wrapperSdk;

+ (void) setAppSecret: (NSString *)secret
{
  appSecret = secret;
}

+ (NSString *) getAppSecret
{
  if (appSecret == nil) {
    NSString * plistPath = [[NSBundle mainBundle] pathForResource:@"MobileCenter-Config" ofType:@"plist"];
    NSDictionary * config = [NSDictionary dictionaryWithContentsOfFile:plistPath];

    appSecret = [config objectForKey:@"AppSecret"];
    // If the AppSecret is not set, we will pass nil to MSMobileCenter which will error out, as expected
  }

  return appSecret;
}

+ (void) configureMobileCenter
{
  if (![MSMobileCenter isConfigured]) {
      MSWrapperSdk * wrapperSdk =
        [[MSWrapperSdk alloc]
            initWithWrapperSdkVersion:@"0.10.0"
            wrapperSdkName:@"mobilecenter.react-native"
            wrapperRuntimeVersion:nil
            liveUpdateReleaseLabel:nil
            liveUpdateDeploymentKey:nil
            liveUpdatePackageHash:nil];
      [self setWrapperSdk:wrapperSdk];
      [MSMobileCenter configureWithAppSecret:[RNMobileCenterShared getAppSecret]];
  }
}

+ (MSWrapperSdk *) getWrapperSdk {
    return wrapperSdk;
}
+ (void) setWrapperSdk:(MSWrapperSdk *)sdk {
    wrapperSdk = sdk;
    [MSMobileCenter setWrapperSdk:sdk];
}

@end
