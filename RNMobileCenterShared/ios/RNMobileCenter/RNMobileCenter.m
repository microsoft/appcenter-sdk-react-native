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
    // If the AppSecret is not set, we will pass nil to MSMobileCenter which will error out, as expected
  }

  return appSecret;
}

+ (void) configureMobileCenter
{
  if (![MSMobileCenter isConfigured]) {
      MSWrapperSdk * wrapperSdk =
        [[MSWrapperSdk alloc]
            initWithWrapperSdkVersion:@"0.5.0"
            wrapperSdkName:@"mobilecenter.react-native"
            wrapperRuntimeVersion:nil
            liveUpdateReleaseLabel:nil
            liveUpdateDeploymentKey:nil
            liveUpdatePackageHash:nil];
      [self setWrapperSdk:wrapperSdk];
      [MSMobileCenter configureWithAppSecret:[RNMobileCenter getAppSecret]];
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

+ (void) setCustomProperties:(NSDictionary*)properties {
    //TODO: check if key is valid (match regular expression pattern ^[a-zA-Z][a-zA-Z0-9]*$)
    //TODO: check if property's value isone of the following datatypes: NSString, NSNumber, BOOL and NSDate.
    MSCustomProperties *customProperties = [MSCustomProperties new];
    //replace this to real values from dict
    [customProperties setString:@"blue" forKey:@"color"];
    [customProperties setNumber:@(10) forKey:@"score"];

    [MSMobileCenter setCustomProperties:customProperties];
}

@end
