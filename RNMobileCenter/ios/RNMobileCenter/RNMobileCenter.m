#import "RNMobileCenter.h"

@implementation RNMobileCenter

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
    // If the AppSecret is not set, we will pass nil to SNMSonoma which will error out, as expected
  }

  return appSecret;
}

+ (void) initializeMobileCenter
{
  if (![MSMobileCenter isInitialized]) {
      MSWrapperSdk * wrapperSdk =
        [[MSWrapperSdk alloc]
            initWithWrapperSdkVersion:@"0.1.0"
            wrapperSdkName:@"mobilecenter.react-native"
            liveUpdateReleaseLabel:nil
            liveUpdateDeploymentKey:nil
            liveUpdatePackageHash:nil];
      [self setWrapperSdk:wrapperSdk];
      [MSMobileCenter start:[RNMobileCenter getAppSecret]];
  }
}

+ (void) setEnabled:(BOOL) enabled
{
  [MSMobileCenter setEnabled:enabled];
}

+ (void) setLogLevel: (MSLogLevel)logLevel
{
  [MSMobileCenter setLogLevel:logLevel];
}

+ (MSLogLevel) logLevel
{
  return [MSMobileCenter logLevel];
}

+ (MSWrapperSdk *) getWrapperSdk {
    return wrapperSdk;
}
+ (void) setWrapperSdk:(MSWrapperSdk *)sdk {
    wrapperSdk = sdk;
    [MSMobileCenter setWrapperSdk:sdk];
}

@end
