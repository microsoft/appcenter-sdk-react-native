// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import <Foundation/Foundation.h>

@class MSACWrapperSdk;

@interface AppCenterReactNativeShared : NSObject

+ (void)setAppSecret:(NSString *)secret;

+ (NSString *)getAppSecret;

+ (void)configureAppCenter;

+ (MSACWrapperSdk *)getWrapperSdk;

+ (void)setWrapperSdk:(MSACWrapperSdk *)sdk;

+ (void)setStartAutomatically:(BOOL)shouldStartAutomatically;

+ (NSDictionary *)getConfiguration;

@end
