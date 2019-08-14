// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

#import "AppDelegate.h"

#import <React/RCTBridge.h>
#import <React/RCTBundleURLProvider.h>
#import <React/RCTRootView.h>

#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>
#import <AppCenterReactNative.h>
#import <AppCenterReactNativeAnalytics.h>
#import <AppCenterReactNativeAuth.h>
#import <AppCenterReactNativeCrashes.h>
#import <AppCenterReactNativePush.h>

@import AppCenter;

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
  [MSAppCenter setLogLevel: MSLogLevelVerbose];
  
  id appSecret = [[NSUserDefaults standardUserDefaults] objectForKey:@"AppSecret"];
  if ([appSecret isKindOfClass:[NSString class]]) {
    [AppCenterReactNativeShared setAppSecret:appSecret];
  }
  
  id startAutomatically = [[NSUserDefaults standardUserDefaults] objectForKey:@"StartAutomatically"];
  if ([startAutomatically isKindOfClass:[NSNumber class]]) {
    [AppCenterReactNativeShared setStartAutomatically:[startAutomatically boolValue]];
  }

  [AppCenterReactNative register]; // Initialize AppCenter
  [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:true]; // Initialize AppCenter Analytics
  [AppCenterReactNativeAuth register]; // Initialize AppCenter Auth
  [AppCenterReactNativeCrashes register]; // Initialize AppCenter Crashes
  [AppCenterReactNativePush register]; // Initialize AppCenter Push

  RCTBridge *bridge = [[RCTBridge alloc] initWithDelegate:self launchOptions:launchOptions];
  RCTRootView *rootView = [[RCTRootView alloc] initWithBridge:bridge
                                                   moduleName:@"TestApp"
                                            initialProperties:nil];

  rootView.backgroundColor = [[UIColor alloc] initWithRed:1.0f green:1.0f blue:1.0f alpha:1];

  self.window = [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];
  UIViewController *rootViewController = [UIViewController new];
  rootViewController.view = rootView;
  self.window.rootViewController = rootViewController;
  [self.window makeKeyAndVisible];
  return YES;
}

- (NSURL *)sourceURLForBridge:(RCTBridge *)bridge
{
#if DEBUG
  return [[RCTBundleURLProvider sharedSettings] jsBundleURLForBundleRoot:@"index" fallbackResource:nil];
#else
  return [[NSBundle mainBundle] URLForResource:@"main" withExtension:@"jsbundle"];
#endif
}

@end
