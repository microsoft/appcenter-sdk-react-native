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
#import <AppCenterData/MSDataError.h>
#import <AppCenterData/MSDictionaryDocument.h>
#import <AppCenterData/MSDocumentWrapper.h>
#import <AppCenterData/MSReadOptions.h>
#import <AppCenterData/MSWriteOptions.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface AppCenterReactNativeData () <RCTBridgeModule>

@end

@implementation AppCenterReactNativeData

static NSString *const kMSDeserializedValueKey = @"deserializedValue";

static NSString *const kMSjsonValueKey = @"jsonValue";

static NSString *const kMSPartitionKey = @"partition";

static NSString *const kMSIDKey = @"id";

static NSString *const kMSETagKey = @"eTag";

static NSString *const kMSLastUpdatedDateKey = @"lastUpdatedDate";

static NSString *const kMSIsFromDeviceCacheKey = @"isFromDeviceCache";

static NSString *const kMSTimeToLiveKey = @"timeToLive";

RCT_EXPORT_MODULE();

+ (void)register {
    [AppCenterReactNativeShared configureAppCenter];
    if ([MSAppCenter isConfigured]) {
        [MSAppCenter startService:[MSData class]];
    }
}

RCT_EXPORT_METHOD(read:(NSString *)documentID
                  partition:(NSString *)partition
                  readOptions:(NSDictionary *)readOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSReadOptions * readOptions = [self getReadOptions:readOptionsMap];
    [MSData readDocumentWithID:documentID
                  documentType:[MSDictionaryDocument class]
                     partition:partition
                   readOptions:readOptions
             completionHandler:[self dataCompletionHandler:@"Failed read" reject:reject resolve:resolve]];
}

RCT_EXPORT_METHOD(create:(NSString *)documentID
                  partition:(NSString *)partition
                  document: (NSDictionary *)documentMap
                  writeOptions:(NSDictionary *)writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject){
    MSWriteOptions * writeOptions = [self getWriteOptions:writeOptionsMap];
    MSDictionaryDocument *document = [[MSDictionaryDocument alloc] initFromDictionary:documentMap];
    [MSData createDocumentWithID:documentID
                        document:document
                       partition:partition
                    writeOptions:writeOptions
               completionHandler:[self dataCompletionHandler:@"Failed create" reject:reject resolve:resolve]];
}

RCT_EXPORT_METHOD(replace:(NSString *)documentID
                  partition:(NSString *)partition
                  document:(NSDictionary *)documentMap
                  writeOptions:(NSDictionary *)writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject){
    MSWriteOptions * writeOptions = [self getWriteOptions:writeOptionsMap];
    MSDictionaryDocument *document = [[MSDictionaryDocument alloc] initFromDictionary:documentMap];
    [MSData replaceDocumentWithID:documentID
                         document:document
                        partition:partition
                     writeOptions:writeOptions
                completionHandler:[self dataCompletionHandler:@"Failed replace" reject:reject resolve:resolve]];
}

RCT_EXPORT_METHOD(remove:(NSString *)documentID
                  partition:(NSString *)partition
                  writeOptions:(NSDictionary* )writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions * writeOptions = [self getWriteOptions:writeOptionsMap];
    [MSData deleteDocumentWithID:documentID
                       partition:partition
                    writeOptions:writeOptions
               completionHandler:[self dataCompletionHandler:@"Failed remove" reject:reject resolve:resolve]];
}

- (void (^)(MSDocumentWrapper * _Nonnull))dataCompletionHandler:(NSString*)errorCode
                                                         reject:(RCTPromiseRejectBlock)reject
                                                        resolve:(RCTPromiseResolveBlock)resolve {
    return ^(MSDocumentWrapper * _Nonnull documentWrapper) {
        NSMutableDictionary *jsDocumentWrapper = [[NSMutableDictionary alloc] init];
        jsDocumentWrapper[kMSjsonValueKey] = documentWrapper.jsonValue;
        jsDocumentWrapper[kMSPartitionKey] = documentWrapper.partition;
        jsDocumentWrapper[kMSIDKey] = documentWrapper.documentId;
        jsDocumentWrapper[kMSETagKey] = documentWrapper.eTag;
        jsDocumentWrapper[kMSLastUpdatedDateKey] = @([documentWrapper.lastUpdatedDate timeIntervalSince1970] * 1000);
        jsDocumentWrapper[kMSIsFromDeviceCacheKey] = [NSNumber numberWithBool:documentWrapper.fromDeviceCache];
        if (documentWrapper.error) {
            MSDataError *dataError = documentWrapper.error;
            [jsDocumentWrapper addEntriesFromDictionary:dataError.userInfo];
            NSError *error = [[NSError alloc] initWithDomain:dataError.domain code:dataError.code userInfo:jsDocumentWrapper];
            reject(@"nope", error.description, error);
            return;
        }
        jsDocumentWrapper[kMSDeserializedValueKey] = [documentWrapper.deserializedValue serializeToDictionary];
        return resolve(jsDocumentWrapper);
    };
}

- (MSReadOptions *)getReadOptions:(NSDictionary *)readOptionsMap {
    MSReadOptions *readOptions;
    if (!readOptionsMap && [readOptionsMap valueForKey:kMSTimeToLiveKey]) {
        readOptions = [[MSReadOptions alloc] initWithDeviceTimeToLive:[[readOptionsMap valueForKey:kMSTimeToLiveKey] integerValue]];
    } else {
        readOptions = [[MSReadOptions alloc] init];
    }
    return readOptions;
}

- (MSWriteOptions *)getWriteOptions:(NSDictionary *)writeOptionsMap {
    MSWriteOptions *writeOptions;
    if (!writeOptionsMap && [writeOptionsMap valueForKey:kMSTimeToLiveKey]) {
        writeOptions = [[MSWriteOptions alloc] initWithDeviceTimeToLive:[[writeOptionsMap valueForKey:kMSTimeToLiveKey] integerValue]];
    } else {
        writeOptions = [[MSWriteOptions alloc] init];
    }
    return writeOptions;
}

@end
