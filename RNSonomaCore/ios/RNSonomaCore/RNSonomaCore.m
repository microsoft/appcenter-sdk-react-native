#import "RNSonomaCore.h"

#import <SonomaCore/SonomaCore.h>


@implementation RNSonomaCore

static NSString *appSecret;

+ (void) setAppSecret: (NSString *)secret
{
  appSecret = secret;
}

+ (NSString *) getAppSecret
{
  if (appSecret == nil) {
    appSecret = [[NSBundle mainBundle] objectForInfoDictionaryKey:"SonomaAppSeret"];
    // TODO: If fetching appsecret fails and it was not set in code, bail out?
  }

  return appSecret;
}

+ (void) initializeSonoma
{
  if (![SNMSonoma isInitialized]) {
    id codePush = NSClassFromString(@"CodePushConfig");
    if (codePush != nil) {
      // Code push is present in the project
      // TODO: find out codepush related info, keeping in mind this could be called very early on.
    }

    // TODO: Add in wrapperSDK once iOS supports that.

    [SNMSonoma start: [RNSonomaCore getAppSecret]]
  }
}

+ (void) setEnabled:(BOOL) enabled
{
  [SNMSonoma setEnabled:enabled];
}

+ (void) setLogLevel: (SNMLogLevel)logLevel
{
  [SNMSonoma setLogLevel:logLevel];
}

+ (SNMLogLevel) getLogLevel
{
  return [SNMSonoma getLogLevel];
}

@end
