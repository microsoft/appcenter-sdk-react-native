/**
 * Copyright (c) 2015-present, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

#import "AppDelegate.h"
#import <RNMobileCenter/RNMobileCenter.h>
#import <RNPush/RNPush.h>
#import <RNAnalytics/RNAnalytics.h>
#import <RNCrashes/RNCrashes.h>

#import <React/RCTBundleURLProvider.h>
#import <React/RCTRootView.h>
@import MobileCenter;

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
  [MSMobileCenter setLogLevel: MSLogLevelVerbose];
  //[MSMobileCenter setServerUrl:@"https://in-integration.dev.avalanch.es"];
  
  NSURL *jsCodeLocation;

  [RNPush register];  // Initialize Mobile Center push

  [RNCrashes register];  // Initialize Mobile Center crashes

  [RNAnalytics registerWithInitiallyEnabled:true];  // Initialize Mobile Center analytics

  [RNMobileCenter register];  // Initialize Mobile Center 

  jsCodeLocation = [[RCTBundleURLProvider sharedSettings] jsBundleURLForBundleRoot:@"index.ios" fallbackResource:nil];

  RCTRootView *rootView = [[RCTRootView alloc] initWithBundleURL:jsCodeLocation
                                                      moduleName:@"TestApp"
                                               initialProperties:nil
                                                   launchOptions:launchOptions];
  rootView.backgroundColor = [[UIColor alloc] initWithRed:1.0f green:1.0f blue:1.0f alpha:1];

  self.window = [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];
  UIViewController *rootViewController = [UIViewController new];
  rootViewController.view = rootView;
  self.window.rootViewController = rootViewController;
  [self.window makeKeyAndVisible];
  return YES;
}

@end
