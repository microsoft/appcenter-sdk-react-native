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
#import <AppCenterData/MSPage.h>
#import <AppCenterData/MSPaginatedDocuments.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>

#import "AppCenterReactNativeRemoteOperationDelegate.h"
#import "AppCenterReactNativeDataUtils.h"

@interface AppCenterReactNativeData () <RCTBridgeModule>

@property (nonatomic, strong) NSMutableDictionary *paginatedDocuments;

@end

@implementation AppCenterReactNativeData

static AppCenterReactNativeRemoteOperationDelegate *remoteOpetationDelegate = nil;

RCT_EXPORT_MODULE();

- (instancetype)init {
    if ((self = [super init])) {
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

//RCT_EXPORT_METHOD(setRemoteOperationDelegate:(RCTPromiseResolveBlock)resolve
//                  rejecter:(RCTPromiseRejectBlock)reject) {
//    resolve(@([MSData setRemoteOperationDelegate:]));
//}

RCT_EXPORT_METHOD(isEnabled:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    resolve(@([MSData isEnabled]));
}

RCT_EXPORT_METHOD(setEnabled:(BOOL)shouldEnable
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    [MSData setEnabled:shouldEnable];
    resolve(nil);
}

RCT_EXPORT_METHOD(read:(NSString *)documentID
                  partition:(NSString *)partition
                  readOptions:(NSDictionary *)readOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSReadOptions *readOptions = [AppCenterReactNativeDataUtils getReadOptions:readOptionsMap];
    [MSData readDocumentWithID:documentID
                  documentType:[MSDictionaryDocument class]
                     partition:partition
                   readOptions:readOptions
             completionHandler:[AppCenterReactNativeDataUtils dataCompletionHandler:kMSReadFailedErrorCode resolver:resolve rejecter:reject]];
}

RCT_EXPORT_METHOD(list:(NSString *)partition
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    [MSData listDocumentsWithType:[MSDictionaryDocument class] partition:partition completionHandler:^(MSPaginatedDocuments* _Nonnull documentWrappers) {
        NSString *paginatedDocumentsId = [[NSUUID UUID] UUIDString];
        _paginatedDocuments[paginatedDocumentsId] = documentWrappers;
        NSMutableDictionary *paginatedDocumentsDict = [[NSMutableDictionary alloc] init];
        NSMutableDictionary *currentPageDict = [[NSMutableDictionary alloc] init];
        MSPage *currentPage = documentWrappers.currentPage;
        if (currentPage.error) {
            [self close:paginatedDocumentsId];
            reject(kMSListFailedErrorCode, currentPage.error.description, currentPage.error);
            return;
        }
        currentPageDict[kMSItemsKey] = [AppCenterReactNativeDataUtils addDocumentsToArray:currentPage.items];
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
    if (!paginatedDocuments) {
        [self close:paginatedDocumentsId];
        reject(kMSListFailedErrorCode, @"No additional pages available", nil);
        return;
    }
    [paginatedDocuments nextPageWithCompletionHandler:^(MSPage* _Nonnull page) {
        NSMutableDictionary *pageMap = [[NSMutableDictionary alloc] init];
        if (page.error) {
            [self close:paginatedDocumentsId];
            reject(kMSListFailedErrorCode, page.error.description, page.error);
            return;
        }
        pageMap[kMSItemsKey] = [AppCenterReactNativeDataUtils addDocumentsToArray:page.items];;
        resolve(pageMap);
    }];
}

RCT_EXPORT_METHOD(close:(NSString *)paginatedDocumentsId) {
    [_paginatedDocuments removeObjectForKey:paginatedDocumentsId];
}

RCT_EXPORT_METHOD(create:(NSString *)documentID
                  partition:(NSString *)partition
                  document: (NSDictionary *)documentMap
                  writeOptions:(NSDictionary *)writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions *writeOptions = [AppCenterReactNativeDataUtils getWriteOptions:writeOptionsMap];
    MSDictionaryDocument *document = [[MSDictionaryDocument alloc] initFromDictionary:documentMap];
    [MSData createDocumentWithID:documentID
                        document:document
                       partition:partition
                    writeOptions:writeOptions
               completionHandler:[AppCenterReactNativeDataUtils dataCompletionHandler:kMSCreateFailedErrorCode resolver:resolve rejecter:reject]];
}

RCT_EXPORT_METHOD(replace:(NSString *)documentID
                  partition:(NSString *)partition
                  document:(NSDictionary *)documentMap
                  writeOptions:(NSDictionary *)writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions *writeOptions = [AppCenterReactNativeDataUtils getWriteOptions:writeOptionsMap];
    MSDictionaryDocument *document = [[MSDictionaryDocument alloc] initFromDictionary:documentMap];
    [MSData replaceDocumentWithID:documentID
                         document:document
                        partition:partition
                     writeOptions:writeOptions
                completionHandler:[AppCenterReactNativeDataUtils dataCompletionHandler:kMSReplaceFailedErrorCode resolver:resolve rejecter:reject]];
}

RCT_EXPORT_METHOD(remove:(NSString *)documentID
                  partition:(NSString *)partition
                  writeOptions:(NSDictionary* )writeOptionsMap
                  resolver:(RCTPromiseResolveBlock)resolve
                  rejecter:(RCTPromiseRejectBlock)reject) {
    MSWriteOptions *writeOptions = [AppCenterReactNativeDataUtils getWriteOptions:writeOptionsMap];
    [MSData deleteDocumentWithID:documentID
                       partition:partition
                    writeOptions:writeOptions
               completionHandler:[AppCenterReactNativeDataUtils dataCompletionHandler:kMSRemoveFailedErrorCode resolver:resolve rejecter:reject]];
}



@end
