// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Foundation;

namespace Microsoft.AppCenter.Data.iOS.Bindings
{
    // @protocol MSSerializableDocument <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSSerializableDocument
    {
        // @required - (instancetype)initFromDictionary:(NSDictionary *)dictionary;
        [Export("initFromDictionary:")]
        MSSerializableDocument Init(NSDictionary dictionary);

        // - (nonnull NSDictionary *)serializeToDictionary
        [Export("serializeToDictionary")]
        NSDictionary SerializeToDictionary();
    }

    // @interface MSDictionaryDocument : NSObject<MSSerializableDocument>
    [BaseType(typeof(MSSerializableDocument))]
    interface MSDictionaryDocument
    {
        // @required - (instancetype)initFromDictionary:(NSDictionary *)dictionary;
        [Export("initFromDictionary:")]
        MSDictionaryDocument Init(NSDictionary dictionary);

        // @property(readonly) NSDictionary *dictionary;
        [Export("dictionary")]
        NSDictionary Dictionary { get; }
    }

    // @interface MSData : MSService
    [BaseType(typeof(NSObject))]
    interface MSData
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // + (void)setTokenExchangeUrl:(NSString *)tokenExchangeUrl;
        [Static]
        [Export("setTokenExchangeUrl:")]
        void SetTokenExchangeUrl(string tokenExchangeUrl);

        // + (void)readWithPartition:(NSString *)partition 
        //                documentId:(NSString*) documentId 
        //              documentType:(Class) documentType 
        //               readOptions:(MSReadOptions *_Nullable)readOptions 
        //         completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("readWithPartition:documentId:documentType:readOptions:completionHandler:")]
        void Read(string partition, string documentId, [NullAllowed] MSReadOptions readOptions, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void)listWithPartition:(NSString *)partition
        //              documentType:(Class) documentType
        //         completionHandler:(MSPaginatedDocumentsCompletionHandler) completionHandler;
        [Static]
        [Export("listWithPartition:documentType:completionHandler:")]
        void List(string partition, MSPaginatedDocumentsCompletionHandler completionHandler);

        // + (void)createWithPartition:(NSString *)partition 
        //                  documentId:(NSString*) documentId 
        //                    document:(id<MSSerializableDocument>)document
        //                writeOptions:(MSWriteOptions *_Nullable)writeOptions 
        //           completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("createWithPartition:documentId:document:writeOptions:completionHandler:")]
        void Create(string partition, string documentId, MSDictionaryDocument document, [NullAllowed] MSWriteOptions writeOptions, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void)replaceWithPartition:(NSString *)partition 
        //                   documentId:(NSString*) documentId 
        //                     document:(id<MSSerializableDocument>)document 
        //                 writeOptions:(MSWriteOptions *_Nullable)writeOptions 
        //            completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("replaceWithPartition:documentId:document:writeOptions:completionHandler:")]
        void Replace(string partition, string documentId, MSDictionaryDocument document, [NullAllowed] MSWriteOptions writeOptions, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void)deleteWithPartition:(NSString *)partition 
        //                  documentId:(NSString*) documentId
        //           completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("deleteWithPartition:documentId:completionHandler:")]
        void Delete(string partition, string documentId, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void)deleteWithPartition:(NSString *)partition 
        //                  documentId:(NSString*) documentId
        //                writeOptions:(MSWriteOptions *_Nullable)writeOptions
        //           completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("deleteWithPartition:documentId:writeOptions:completionHandler:")]
        void Delete(string partition, string documentId, [NullAllowed] MSWriteOptions writeOptions, MSDocumentWrapperCompletionHandler completionHandler);
    }

    // @interface MSDocumentWrapper : NSObject
    [BaseType(typeof(NSObject))]
    interface MSDocumentWrapper
    {
        // @property(nonatomic, strong, readonly) NSString *jsonValue;
        [Export("jsonValue")]
        string JsonValue { get; }

        // @property(nonatomic, strong, readonly) id<MSSerializableDocument> deserializedValue;
        [Export("deserializedValue")]
        MSDictionaryDocument DeserializedValue { get; }

        // @property(nonatomic, strong, readonly) NSString *partition;
        [Export("partition")]
        string Partition { get; }

        // @property(nonatomic, strong, readonly) NSString *documentId;
        [Export("documentId")]
        string DocumentId { get; }

        // @property(nonatomic, strong, readonly) NSString *eTag;
        [Export("eTag")]
        string ETag { get; }

        // @property(nonatomic, strong, readonly) NSDate *lastUpdatedDate;
        [Export("lastUpdatedDate")]
        NSDate LastUpdatedDate { get; }

        // @property(nonatomic, strong, readonly) MSDataError *error;
        [Export("error")]
        MSDataError Error { get; }

        // @property(nonatomic, readonly) BOOL fromDeviceCache;
        [Export("fromDeviceCache")]
        bool FromDeviceCache { get; set; }
    }

    // @interface MSPaginatedDocuments : NSObject
    [BaseType(typeof(NSObject))]
    interface MSPaginatedDocuments
    {
        // - (BOOL)hasNextPage;
        [Export("hasNextPage")]
        bool HasNextPage();

        // - (MSPage *)currentPage;
        [Export("currentPage")]
        MSPage CurrentPage();

        // - (void)nextPageWithCompletionHandler:(void (^)(MSPage *page))completionHandler;
        [Export("nextPageWithCompletionHandler:")]
        void NextPage(MSPageCompletionHandler completionHandler);
    }

    // @interface MSPage : NSObject
    [BaseType(typeof(NSObject))]
    interface MSPage
    {
        // @property(readonly) NSArray<MSDocumentWrapper *> *items;
        [Export("items")]
        MSDocumentWrapper[] Items { get; }

        // @property(readonly) MSDataError *error;
        [Export("error")]
        MSDataError Error { get; }
    }

    // @interface MSDataError : NSObject
    [BaseType(typeof(NSObject))]
    interface MSDataError
    {
        // @property(nonatomic, strong, readonly) NSError *error;
        [Export("error")]
        NSError Error { get; }

        // @property(nonatomic, readonly) NSInteger errorCode;
        [Export("errorCode")]
        IntPtr errorCode { get; }
    }

    // @interface MSBaseOptions : NSObject
    [BaseType(typeof(NSObject))]
    interface MSBaseOptions
    {
        // @property NSInteger deviceTimeToLive;
        [Export("deviceTimeToLive")]
        long DeviceTimeToLive { get; set; }
    }

    // @interface MSReadOptions : MSBaseOptions
    [BaseType(typeof(MSBaseOptions))]
    interface MSReadOptions : MSBaseOptions
    {
    }

    // @interface MSWriteOptions : MSBaseOptions
    [BaseType(typeof(MSBaseOptions))]
    interface MSWriteOptions : MSBaseOptions
    {
    }

    // typedef void (^MSDocumentWrapperCompletionHandler)(MSDocumentWrapper *document);
    delegate void MSDocumentWrapperCompletionHandler(MSDocumentWrapper document);

    // typedef void (^MSPaginatedDocumentsCompletionHandler)(MSPaginatedDocuments *documents);
    delegate void MSPaginatedDocumentsCompletionHandler(MSPaginatedDocuments documents);

    // void (^)(MSPage *page)
    delegate void MSPageCompletionHandler(MSPage page);
}

