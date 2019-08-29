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

@interface AppCenterReactNativeDataUtils : NSObject

+ (void)addDocumentWrapperMetaData:(NSMutableDictionary *)jsDocumentWrapper document:(MSDocumentWrapper *)document;
+ (NSMutableArray *)addDocumentsToArray:(NSArray<MSDocumentWrapper *> *)documents;
+ (MSReadOptions *)getReadOptions:(NSDictionary *)readOptionsMap;
+ (MSWriteOptions *)getWriteOptions:(NSDictionary *)writeOptionsMap;
+ (void (^)(MSDocumentWrapper *_Nonnull))dataCompletionHandler:(NSString *)errorCode
                                                      resolver:(RCTPromiseResolveBlock)resolve
                                                      rejecter:(RCTPromiseRejectBlock)reject;

@end
