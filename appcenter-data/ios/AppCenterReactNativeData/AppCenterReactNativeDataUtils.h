// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import <Foundation/Foundation.h>

#import <React/RCTBridgeModule.h>

#import <AppCenter/MSAppCenter.h>
#import <AppCenterData/MSData.h>
#import <AppCenterData/MSDataError.h>
#import <AppCenterData/MSDictionaryDocument.h>
#import <AppCenterData/MSDocumentWrapper.h>
#import <AppCenterData/MSReadOptions.h>
#import <AppCenterData/MSWriteOptions.h>

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

@interface AppCenterReactNativeDataUtils : NSObject

+ (void)addDocumentWrapperMetaData:(NSMutableDictionary *)jsDocumentWrapper document:(MSDocumentWrapper *)document;
+ (NSMutableArray *)addDocumentsToArray:(NSArray<MSDocumentWrapper *> *)documents;
+ (MSReadOptions *)getReadOptions:(NSDictionary *)readOptionsMap;
+ (MSWriteOptions *)getWriteOptions:(NSDictionary *)writeOptionsMap;
+ (void (^)(MSDocumentWrapper* _Nonnull))dataCompletionHandler:(NSString *)errorCode resolver:(RCTPromiseResolveBlock)resolve rejecter:(RCTPromiseRejectBlock)reject;

@end
