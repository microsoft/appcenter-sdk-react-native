// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNativeDataUtils.h"
#import "AppCenterReactNativeDataContants.h"

@implementation AppCenterReactNativeDataUtils

+ (void)addDocumentWrapperMetaData:(NSMutableDictionary *)jsDocumentWrapper
                          document:(MSDocumentWrapper *)document {
  jsDocumentWrapper[kMSETagKey] = document.eTag ? document.eTag : [NSNull null];
  jsDocumentWrapper[kMSIDKey] =
      document.documentId ? document.documentId : [NSNull null];
  jsDocumentWrapper[kMSPartitionKey] =
      document.partition ? document.partition : [NSNull null];
  jsDocumentWrapper[kMSLastUpdatedDateKey] =
      document.lastUpdatedDate
          ? @([document.lastUpdatedDate timeIntervalSince1970] * 1000)
          : [NSNull null];
  jsDocumentWrapper[kMSIsFromDeviceCacheKey] =
      [NSNumber numberWithBool:document.fromDeviceCache];
  jsDocumentWrapper[kMSjsonValueKey] =
      document.jsonValue ? document.jsonValue : [NSNull null];
}

+ (NSMutableArray *)addDocumentsToArray:
    (NSArray<MSDocumentWrapper *> *)documents {
  NSMutableArray *itemsArray = [[NSMutableArray alloc] init];
  for (MSDocumentWrapper *document in documents) {
    NSMutableDictionary *jsDocumentWrapper = [[NSMutableDictionary alloc] init];
    [AppCenterReactNativeDataUtils addDocumentWrapperMetaData:jsDocumentWrapper
                                                     document:document];
    if (document.error) {
      NSMutableDictionary *errorDict = [[NSMutableDictionary alloc] init];
      errorDict[kMSMessageKey] = document.error.description;
      jsDocumentWrapper[kMSErrorKey] = errorDict;
    } else {
      jsDocumentWrapper[kMSErrorKey] = nil;
    }
    jsDocumentWrapper[kMSDeserializedValueKey] =
        document.deserializedValue
            ? [document.deserializedValue serializeToDictionary]
            : [NSNull null];
    [itemsArray addObject:jsDocumentWrapper];
  }
  return itemsArray;
}

+ (MSReadOptions *)getReadOptions:(NSDictionary *)readOptionsMap {
  MSReadOptions *readOptions;
  if ([readOptionsMap valueForKey:kMSTimeToLiveKey]) {
    readOptions = [[MSReadOptions alloc]
        initWithDeviceTimeToLive:[[readOptionsMap valueForKey:kMSTimeToLiveKey]
                                     integerValue]];
  } else {
    readOptions = [[MSReadOptions alloc] init];
  }
  return readOptions;
}

+ (MSWriteOptions *)getWriteOptions:(NSDictionary *)writeOptionsMap {
  MSWriteOptions *writeOptions;
  if ([writeOptionsMap valueForKey:kMSTimeToLiveKey]) {
    writeOptions = [[MSWriteOptions alloc]
        initWithDeviceTimeToLive:[[writeOptionsMap valueForKey:kMSTimeToLiveKey]
                                     integerValue]];
  } else {
    writeOptions = [[MSWriteOptions alloc] init];
  }
  return writeOptions;
}

+ (void (^)(MSDocumentWrapper *_Nonnull))
    dataCompletionHandler:(NSString *)errorCode
                 resolver:(RCTPromiseResolveBlock)resolve
                 rejecter:(RCTPromiseRejectBlock)reject {
  return ^(MSDocumentWrapper *_Nonnull documentWrapper) {
    NSMutableDictionary *jsDocumentWrapper = [[NSMutableDictionary alloc] init];
    [AppCenterReactNativeDataUtils addDocumentWrapperMetaData:jsDocumentWrapper
                                                     document:documentWrapper];
    if (documentWrapper.error) {
      MSDataError *dataError = documentWrapper.error;
      [jsDocumentWrapper addEntriesFromDictionary:dataError.userInfo];
      NSError *error = [[NSError alloc] initWithDomain:dataError.domain
                                                  code:dataError.code
                                              userInfo:jsDocumentWrapper];
      reject(errorCode, dataError.description, error);
      return;
    }
    jsDocumentWrapper[kMSDeserializedValueKey] =
        documentWrapper.deserializedValue
            ? [documentWrapper.deserializedValue serializeToDictionary]
            : [NSNull null];
    resolve(jsDocumentWrapper);
  };
}

@end
