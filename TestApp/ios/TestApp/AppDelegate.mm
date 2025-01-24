#import "AppDelegate.h"

#import "TestAppNative.h"
#import <React/RCTBundleURLProvider.h>
#import <AppCenterReactNative.h>
#import <AppCenterReactNativeAnalytics.h>
#import <AppCenterReactNativeCrashes.h>

#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>
#import <AppCenter/AppCenter.h>
#import <AppCenterAnalytics/AppCenterAnalytics.h>

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
  self.moduleName = @"TestApp";
  // You can add your custom initial props in the dictionary below.
  // They will be passed down to the ViewController used by React Native.
  self.initialProps = @{};

  [MSACAppCenter setLogLevel:MSACLogLevelVerbose];

  id appSecret = [[NSUserDefaults standardUserDefaults] objectForKey:@"AppSecret"];
  if ([appSecret isKindOfClass:[NSString class]]) {
    [AppCenterReactNativeShared setAppSecret:appSecret];
  }

  id startAutomatically = [[NSUserDefaults standardUserDefaults] objectForKey:@"StartAutomatically"];
  if ([startAutomatically isKindOfClass:[NSNumber class]]) {
    [AppCenterReactNativeShared setStartAutomatically:[startAutomatically boolValue]];
  }
  BOOL sessionAuto = [[NSUserDefaults standardUserDefaults] boolForKey:@"ManualSessionTrackerEnabled"];
  if (sessionAuto) {
    [MSACAnalytics enableManualSessionTracker];
  }

  [AppCenterReactNative register];
  [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:true];
  [AppCenterReactNativeCrashes registerWithAutomaticProcessing];

  return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

- (NSURL *)sourceURLForBridge:(RCTBridge *)bridge
{
  return [self getBundleURL];
}

- (NSURL *)getBundleURL
{
#if DEBUG
  return [[RCTBundleURLProvider sharedSettings] jsBundleURLForBundleRoot:@"index"];
#else
  return [[NSBundle mainBundle] URLForResource:@"main" withExtension:@"jsbundle"];
#endif
}

@end
