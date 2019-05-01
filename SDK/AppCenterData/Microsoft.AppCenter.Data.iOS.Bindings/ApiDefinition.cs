// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Foundation;
using ObjCRuntime;

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
    interface MSDictionaryDocument : MSSerializableDocument
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

        // + (void)readDocumentWithID:(NSString *)documentID
        //               documentType:(Class)documentType
        //                  partition:(NSString *)partition
        //                readOptions:(MSReadOptions *_Nullable)readOptions
        //          completionHandler:(MSDocumentWrapperCompletionHandler)completionHandler;
        [Static]
        [Export("readDocumentWithID:documentType:partition:readOptions:completionHandler:")]
        void Read(string documentId, Class documentType, string partition, [NullAllowed] MSReadOptions readOptions, MSDocumentWrapperCompletionHandler completionHandler);

        // (void)listDocumentsWithType:(Class)documentType
        //                   partition:(NSString*) partition
        //           completionHandler:(MSPaginatedDocumentsCompletionHandler) completionHandler;
        [Static]
        [Export("listDocumentsWithType:partition:completionHandler:")]
        void List(Class documentType, string partition, MSPaginatedDocumentsCompletionHandler completionHandler);

        // + (void) createDocumentWithID:(NSString*) documentID
        //                      document:(id<MSSerializableDocument>) document
        //                     partition:(NSString*) partition
        //                  writeOptions:(MSWriteOptions* _Nullable) writeOptions
        //             completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("createDocumentWithID:document:partition:writeOptions:completionHandler:")]
        void Create(string documentId, MSSerializableDocument document, string partition, [NullAllowed] MSWriteOptions writeOptions, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void) replaceDocumentWithID:(NSString*) documentID
        //                       document:(id<MSSerializableDocument>) document
        //                      partition:(NSString*) partition
        //                   writeOptions:(MSWriteOptions* _Nullable) writeOptions
        //              completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("replaceDocumentWithID:document:partition:writeOptions:completionHandler:")]
        void Replace(string documentId, MSSerializableDocument document, string partition, [NullAllowed] MSWriteOptions writeOptions, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void) deleteDocumentWithID:(NSString*) documentID
        //                     partition:(NSString*) partition
        //             completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("deleteDocumentWithID:partition:completionHandler:")]
        void Delete(string documentId, string partition, MSDocumentWrapperCompletionHandler completionHandler);

        // + (void)deleteDocumentWithID:(NSString *)partition 
        //                    partition:(NSString*) documentId
        //                 writeOptions:(MSWriteOptions *_Nullable)writeOptions
        //            completionHandler:(MSDocumentWrapperCompletionHandler) completionHandler;
        [Static]
        [Export("deleteDocumentWithID:partition:writeOptions:completionHandler:")]
        void Delete(string documentId, string partition, [NullAllowed] MSWriteOptions writeOptions, MSDocumentWrapperCompletionHandler completionHandler);
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

    // @interface MSDataError : NSError
    [BaseType(typeof(NSError))]
    interface MSDataError
    {
        // - (NSError *)innerError;
        [Export("innerError")]
        NSError InnerError { get; }
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

