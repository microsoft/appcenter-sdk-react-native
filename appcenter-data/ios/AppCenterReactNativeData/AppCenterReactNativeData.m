// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNativeData.h"

// Support React Native headers both in the React namespace, where they are in
// RN version 0.40+, and no namespace, for older versions of React Native
#if __has_include(<React/RCTAssert.h>)
#import <React/RCTAssert.h>
#import <React/RCTBridgeModule.h>
#import <React/RCTConvert.h>
#import <React/RCTEventDispatcher.h>
#import <React/RCTRootView.h>
#import <React/RCTUtils.h>
#else
#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"
#endif

#import <AppCenter/MSAppCenter.h>
#import <AppCenterData/MSData.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface AppCenterReactNativeData () <RCTBridgeModule>

@end

@implementation AppCenterReactNativeData

RCT_EXPORT_MODULE();

+ (void)register{
    [AppCenterReactNativeShared configureAppCenter];
    if ([MSAppCenter isConfigured]) {
        [MSAppCenter startService:[MSData class]];
    }
}

@end
