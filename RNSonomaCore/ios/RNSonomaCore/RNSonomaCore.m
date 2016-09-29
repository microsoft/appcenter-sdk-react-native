#import "RNSonomaCore.h"

#import <SonomaCore/SonomaCore.h>


@implementation RNSonomaCore

static NSString *appSecret;

+ (void) setAppSecret: (NSString *)secret
{
  appSecret = secret;
}

+ (void) initializeSonoma
{
  if (![SNMSonoma isInitialized]) {
    if (appSecret == nil) {
      // TODO: Fetch app secret from plist
      appSecret = @"abc123";

      // TODO: If fetching appsecret fails and it was not set in code, bail out?
    }

    id codePush = NSClassFromString(@"CodePushConfig");
    if (codePush != nil) {
      // Code push is present in the project
      // TODO: find out codepush related info, keeping in mind this could be called very early on.
    }

    // TODO: Add in wrapperSDK once iOS supports that.

    [SNMSonoma start: appSecret]
  }
}

@end
