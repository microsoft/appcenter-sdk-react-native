// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNativeAuth.h"

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
#import <AppCenterAuth/MSAuth.h>
#import <AppCenterAuth/MSUserInformation.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface AppCenterReactNativeAuth () <RCTBridgeModule>

@end

@implementation AppCenterReactNativeAuth

static NSString *const kMSAccountId = @"accountId";
static NSString *const kMSAccessToken = @"accessToken";
static NSString *const kMSIdToken = @"idToken";

RCT_EXPORT_MODULE();

+ (void)register {
    [AppCenterReactNativeShared configureAppCenter];
    if ([MSAppCenter isConfigured]) {
        [MSAppCenter startService:[MSAuth class]];
    }
}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    resolve([NSNumber numberWithBool:[MSAuth isEnabled]]);
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)shouldEnable
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    [MSAuth setEnabled:shouldEnable];
    resolve(nil);
}

RCT_EXPORT_METHOD(signIn:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    [MSAuth signInWithCompletionHandler:^(MSUserInformation * _Nullable userInformation, NSError * _Nullable error) {
        if (!error) {
            
            /* Sign-in succeeded, convert native result to a JavaScript result. */
            NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
            dict[kMSAccountId] = userInformation.accountId;
            dict[kMSAccessToken] = userInformation.accessToken;
            dict[kMSIdToken] = userInformation.idToken;
            resolve(dict);
        } else {
            reject(@"sign_in_failed", @"Sign-in failed", error);
        }
    }];
}

RCT_EXPORT_METHOD(signOut) {
    [MSAuth signOut];
}

@end
