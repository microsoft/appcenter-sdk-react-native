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
#import <AppCenterData/MSPaginatedDocuments.h>
#import <AppCenterData/MSPage.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

@interface AppCenterReactNativeData () <RCTBridgeModule>

@property (nonatomic) NSMutableDictionary *paginatedDocuments;

@end

@implementation AppCenterReactNativeData

static NSString *const kMSReadFailedErrorCode = @"ReadFailed";

static NSString *const kMSListFailedErrorCode = @"ListFailed";

static NSString *const kMSCreateFailedErrorCode = @"CreateFailed";

static NSString *const kMSReplaceFailedErrorCode = @"ReplaceFailed";

static NSString *const kMSRemoveFailedErrorCode = @"RemoveFailed";

static NSString *const kMSDeserializedValueKey = @"deserializedValue";

static NSString *const kMSjsonValueKey = @"jsonValue";

static NSString *const kMSETagKey = @"eTag";

static NSString *const kMSLastUpdatedDateKey = @"lastUpdatedDate";

static NSString *const kMSIsFromDeviceCacheKey = @"isFromDeviceCache";

static NSString *const kMSIDKey = @"id";

static NSString *const kMSPartitionKey = @"partition";

static NSString *const kMSTimeToLiveKey = @"timeToLive";

static NSString *const kMSErrorKey = @"error";

static NSString *const kMSPaginatedDocumentsIDKey = @"paginatedDocumentsId";

static NSString *const kMSItemsKey = @"items";

static NSString *const kMSCurrentPageKey = @"currentPage";

static NSString *const kMSMessageKey = @"message";

RCT_EXPORT_MODULE();

- (instancetype)init {
    if((self = [super init])) {
        _paginatedDocuments = [[NSMutableDictionary alloc] init];
    }
    return self;
}

+ (BOOL)requiresMainQueueSetup {
    return NO;
}

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
    MSReadOptions *readOptions = [AppCenterReactNativeData getReadOptions:readOptionsMap];
    [MSData readDocumentWithID:documentID
                  documentType:[MSDictionaryDocument class]
                     partition:partition
                   readOptions:readOptions
             completionHandler:[AppCenterReactNativeData dataCompletionHandler:kMSReadFailedErrorCode resolver:resolve rejecter:reject]];
}

RCT_EXPORT_METHOD(list:(NSString *)partition
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    [MSData listDocumentsWithType:[MSDictionaryDocument class] partition:partition completionHandler:^(MSPaginatedDocuments* _Nonnull documentWrappers) {
        NSString *paginatedDocumentsId = [[NSUUID UUID] UUIDString];
        _paginatedDocuments[paginatedDocumentsId] = documentWrappers;
        NSMutableDictionary *paginatedDocumentsDict = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *currentPageDict = [[NSMutableDictionary alloc] init];
        NSMutableArray *itemsArray = [[NSMutableArray alloc] init];
        MSPage *currentPage = documentWrappers.currentPage;
        if(currentPage.error) {
            reject(kMSListFailedErrorCode, currentPage.error.description, currentPage.error);
            return;
        }
        NSArray<MSDocumentWrapper *> *documents = currentPage.items;
        for(MSDocumentWrapper *document in documents) {
            [AppCenterReactNativeData addDocumentToNSMutableArray:itemsArray document:document];
        }
        currentPageDict[kMSItemsKey] = itemsArray;
        paginatedDocumentsDict[kMSCurrentPageKey] = currentPageDict;
        paginatedDocumentsDict[kMSPaginatedDocumentsIDKey] = paginatedDocumentsId;
        resolve(paginatedDocumentsDict);
    }];
}

RCT_EXPORT_METHOD(hasNextPage:(NSString *)paginatedDocumentsId
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSPaginatedDocuments *paginatedDocuments = _paginatedDocuments[paginatedDocumentsId];
    if (!paginatedDocuments || ![paginatedDocuments hasNextPage]) {
        [self close:paginatedDocumentsId];
        resolve(@(NO));
    } else {
        resolve(@(YES));
    }
}

RCT_EXPORT_METHOD(getNextPage:(NSString *)paginatedDocumentsId
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSPaginatedDocuments *paginatedDocuments = _paginatedDocuments[paginatedDocumentsId];
    if (!paginatedDocuments || ![paginatedDocuments hasNextPage]) {
        [self close:paginatedDocumentsId];
        resolve(nil);
        return;
    }
    [paginatedDocuments nextPageWithCompletionHandler:^(MSPage* _Nonnull page) {
        NSMutableDictionary *pageMap = [[NSMutableDictionary alloc] init];
        NSMutableArray *itemsArray = [[NSMutableArray alloc] init];
        if(page.error) {
            reject(kMSListFailedErrorCode, page.error.description, page.error);
            return;
        }
        NSArray<MSDocumentWrapper *> *documents = page.items;
        for(MSDocumentWrapper *document in documents) {
            [AppCenterReactNativeData addDocumentToNSMutableArray:itemsArray document:document];
        }
        pageMap[kMSItemsKey] = itemsArray;
        resolve(pageMap);
    }];
}

RCT_EXPORT_METHOD(close:(NSString *) paginatedDocumentsId) {
    [_paginatedDocuments removeObjectForKey:paginatedDocumentsId];
}

RCT_EXPORT_METHOD(create:(NSString *)documentID
                  partition:(NSString *)partition
                  document: (NSDictionary *)documentMap
                  writeOptions:(NSDictionary *)writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions *writeOptions = [AppCenterReactNativeData getWriteOptions:writeOptionsMap];
    MSDictionaryDocument *document = [[MSDictionaryDocument alloc] initFromDictionary:documentMap];
    [MSData createDocumentWithID:documentID
                        document:document
                       partition:partition
                    writeOptions:writeOptions
               completionHandler:[AppCenterReactNativeData dataCompletionHandler:kMSCreateFailedErrorCode resolver:resolve rejecter:reject]];
}

RCT_EXPORT_METHOD(replace:(NSString *)documentID
                  partition:(NSString *)partition
                  document:(NSDictionary *)documentMap
                  writeOptions:(NSDictionary *)writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions *writeOptions = [AppCenterReactNativeData getWriteOptions:writeOptionsMap];
    MSDictionaryDocument *document = [[MSDictionaryDocument alloc] initFromDictionary:documentMap];
    [MSData replaceDocumentWithID:documentID
                         document:document
                        partition:partition
                     writeOptions:writeOptions
                completionHandler:[AppCenterReactNativeData dataCompletionHandler:kMSReplaceFailedErrorCode resolver:resolve rejecter:reject]];
}

RCT_EXPORT_METHOD(remove:(NSString *)documentID
                  partition:(NSString *)partition
                  writeOptions:(NSDictionary* )writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions *writeOptions = [AppCenterReactNativeData getWriteOptions:writeOptionsMap];
    [MSData deleteDocumentWithID:documentID
                       partition:partition
                    writeOptions:writeOptions
               completionHandler:[AppCenterReactNativeData dataCompletionHandler:kMSRemoveFailedErrorCode resolver:resolve rejecter:reject]];
}

+ (void (^)(MSDocumentWrapper* _Nonnull))dataCompletionHandler:(NSString *)errorCode
                                                      resolver:(RCTPromiseResolveBlock)resolve
                                                      rejecter:(RCTPromiseRejectBlock)reject {
    return ^(MSDocumentWrapper* _Nonnull documentWrapper) {
        NSMutableDictionary *jsDocumentWrapper = [[NSMutableDictionary alloc] init];
        [AppCenterReactNativeData addDocumentWrapperMetaData:jsDocumentWrapper document:documentWrapper];
        if (documentWrapper.error) {
            MSDataError *dataError = documentWrapper.error;
            [jsDocumentWrapper addEntriesFromDictionary:dataError.userInfo];
            NSError *error = [[NSError alloc] initWithDomain:dataError.domain code:dataError.code userInfo:jsDocumentWrapper];
            reject(errorCode, dataError.description, error);
            return;
        }
        jsDocumentWrapper[kMSDeserializedValueKey] = documentWrapper.deserializedValue ? [documentWrapper.deserializedValue serializeToDictionary] : [NSNull null];
        resolve(jsDocumentWrapper);
    };
}

+ (void)addDocumentWrapperMetaData:(NSMutableDictionary *)jsDocumentWrapper
                          document:(MSDocumentWrapper *)document {
    jsDocumentWrapper[kMSETagKey] = document.eTag ? document.eTag : [NSNull null];
    jsDocumentWrapper[kMSIDKey] = document.documentId ? document.documentId : [NSNull null];
    jsDocumentWrapper[kMSPartitionKey] = document.partition ? document.partition : [NSNull null];
    jsDocumentWrapper[kMSLastUpdatedDateKey] = @([document.lastUpdatedDate timeIntervalSince1970] * 1000);
    jsDocumentWrapper[kMSIsFromDeviceCacheKey] = [NSNumber numberWithBool:document.fromDeviceCache];
    jsDocumentWrapper[kMSjsonValueKey] = document.jsonValue ? document.jsonValue : [NSNull null];
}

+ (void)addDocumentToNSMutableArray:(NSMutableArray *)itemsArray
                           document:(MSDocumentWrapper *)document {
    NSMutableDictionary *jsDocumentWrapper = [[NSMutableDictionary alloc] init];
    [AppCenterReactNativeData addDocumentWrapperMetaData:jsDocumentWrapper document:document];
    if (document.error) {
        NSMutableDictionary *errorDict = [[NSMutableDictionary alloc] init];
        errorDict[kMSMessageKey] = document.error.description;
        jsDocumentWrapper[kMSErrorKey] = errorDict;
    } else {
        jsDocumentWrapper[kMSErrorKey] = nil;
    }
    jsDocumentWrapper[kMSDeserializedValueKey] = document.deserializedValue ? [document.deserializedValue serializeToDictionary] : [NSNull null];
    [itemsArray addObject:jsDocumentWrapper];
}

+ (MSReadOptions *)getReadOptions:(NSDictionary *)readOptionsMap {
    MSReadOptions *readOptions;
    if ([readOptionsMap valueForKey:kMSTimeToLiveKey]) {
        readOptions = [[MSReadOptions alloc] initWithDeviceTimeToLive:[[readOptionsMap valueForKey:kMSTimeToLiveKey] integerValue]];
    } else {
        readOptions = [[MSReadOptions alloc] init];
    }
    return readOptions;
}

+ (MSWriteOptions*)getWriteOptions:(NSDictionary *)writeOptionsMap {
    MSWriteOptions *writeOptions;
    if ([writeOptionsMap valueForKey:kMSTimeToLiveKey]) {
        writeOptions = [[MSWriteOptions alloc] initWithDeviceTimeToLive:[[writeOptionsMap valueForKey:kMSTimeToLiveKey] integerValue]];
    } else {
        writeOptions = [[MSWriteOptions alloc] init];
    }
    return writeOptions;
}

@end
