// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using System;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AppCenter.Data.iOS.Bindings;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Data
{
    public static class ConversionExtensions
    {
        public static DocumentWrapper<T> ToDocumentWrapper<T>(this MSDocumentWrapper documentWrapper)
        {
            var deserializedValue = JsonConvert.DeserializeObject<T>(documentWrapper.JsonValue);
            return new DocumentWrapper<T>
            {
                DeserializedValue = deserializedValue,
                Partition = documentWrapper.Partition,
                ETag = documentWrapper.ETag,
                Id = documentWrapper.DocumentId,
                LastUpdatedDate = (DateTime)documentWrapper.LastUpdatedDate,
                IsFromDeviceCache = documentWrapper.FromDeviceCache
            };
        }

        public static MSReadOptions ToMSReadOptions(this ReadOptions readOptions)
        {
            return new MSReadOptions
            {
                DeviceTimeToLive = readOptions.DeviceTimeToLive.Ticks
            };
        }

        public static MSWriteOptions ToMSWriteOptions(this WriteOptions writeOptions)
        {
            return new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };
        }

        public static Page<T> ToPage<T>(this MSPage msPage)
        {
            if (msPage.Error != null)
            {
                throw msPage.Error.ToDataException();
            }
            return new Page<T>
            {
                Items = msPage.Items.OfType<MSDocumentWrapper>().ToList()
                .Cast<MSDocumentWrapper>()
                .Select(i => i.ToDocumentWrapper<T>()).ToList()
            };
        }

        public static MSDictionaryDocument ToMSDocument<T>(this T document)
        {
            var dic = JsonConvert.DeserializeObject<NSDictionary>(JsonConvert.SerializeObject(document));
            return new MSDictionaryDocument().Init(dic);
        }

        public static PaginatedDocuments<T> ToPaginatedDocuments<T>(this MSPaginatedDocuments mSPaginatedDocuments)
        {
            return new PaginatedDocuments<T>(mSPaginatedDocuments);
        }

        public static DocumentMetadata ToDocumentMetadata(this MSDocumentWrapper documentWrapper)
        {
            var doc = new DocumentMetadata();
            doc.ETag = documentWrapper.ETag;
            doc.Id = documentWrapper.DocumentId;
            doc.Partition = documentWrapper.Partition;
            return doc;
        }

        public static DataException ToDataException(this MSDataError error, MSDocumentWrapper msDocumentWrapper = null) 
        {
            var exception = new NSErrorException(error.Error);
            var dataException = new DataException(exception.Message, exception);
            if (msDocumentWrapper != null) 
            {
                dataException.DocumentMetadata = msDocumentWrapper.ToDocumentMetadata();
            }
            return dataException;
        }
    }
}
