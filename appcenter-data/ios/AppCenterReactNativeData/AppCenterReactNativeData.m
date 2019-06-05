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
                 dict[@"isFromDeviceCache"] = [NSNumber numberWithBool:document.fromDeviceCache];
                 dict[@"jsonValue"] = document.jsonValue;
                 dict[@"eTag"] = document.eTag;
                 dict[@"partition"] = document.partition;
                 dict[@"lastUpdatedDate"] = @([document.lastUpdatedDate timeIntervalSince1970] * 1000);
                 dict[@"deserializedValue"] = document.deserializedValue.serializeToDictionary;
                 return resolve(dict);
             }];
}

@end
