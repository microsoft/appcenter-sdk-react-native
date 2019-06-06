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
#import <AppCenterData/MSDocumentWrapper.h>
#import <AppCenterData/MSDictionaryDocument.h>

@interface AppCenterReactNativeData () <RCTBridgeModule>

@end

@implementation AppCenterReactNativeData

static NSString *const kMSDeserializedValue = @"deserializedValue";
static NSString *const kMSjsonValue = @"jsonValue";
static NSString *const kMSPartition = @"partition";
static NSString *const kMSId = @"id";
static NSString *const kMSETag = @"eTag";
static NSString *const kMSLastUpdatedDate = @"lastUpdatedDate";
static NSString *const kMSIsFromDeviceCache = @"isFromDeviceCache";

RCT_EXPORT_MODULE();

+ (void)register {
    [AppCenterReactNativeShared configureAppCenter];
    if ([MSAppCenter isConfigured]) {
        [MSAppCenter startService:[MSData class]];
    }
}

RCT_EXPORT_METHOD(read:(NSString *)documentId
                  partition:(NSString *) partition
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject){
    [MSData readDocumentWithID: documentId
                  documentType:[MSDictionaryDocument class]
                     partition: partition
             completionHandler:^(MSDocumentWrapper *_Nonnull document) {
                 NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
                 dict[kMSDeserializedValue] = document.deserializedValue.serializeToDictionary;
                 dict[kMSjsonValue] = document.jsonValue;
                 dict[kMSPartition] = document.partition;
                 dict[kMSId] = document.documentId;
                 dict[kMSETag] = document.eTag;
                 dict[kMSLastUpdatedDate] = @([document.lastUpdatedDate timeIntervalSince1970] * 1000);
                 dict[kMSIsFromDeviceCache] = [NSNumber numberWithBool:document.fromDeviceCache];
                 return resolve(dict);
             }];
}

@end
