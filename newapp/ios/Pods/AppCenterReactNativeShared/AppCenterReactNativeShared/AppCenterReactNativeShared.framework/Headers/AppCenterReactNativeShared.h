// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import <Foundation/Foundation.h>

@class MSWrapperSdk;

@interface AppCenterReactNativeShared : NSObject

+ (void)setAppSecret:(NSString *)secret;

+ (NSString *)getAppSecret;

+ (void)configureAppCenter;

+ (MSWrapperSdk *)getWrapperSdk;

+ (void)setWrapperSdk:(MSWrapperSdk *)sdk;

+ (void)setStartAutomatically:(BOOL)shouldStartAutomatically;

+ (NSDictionary *)getConfiguration;

@end
