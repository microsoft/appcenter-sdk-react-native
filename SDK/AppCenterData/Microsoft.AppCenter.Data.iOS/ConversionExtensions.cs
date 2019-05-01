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
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DocumentWrapper<T> ToDocumentWrapper<T>(this MSDocumentWrapper documentWrapper)
        {
            var jsonValue = documentWrapper.JsonValue;
            var deserializedValue = jsonValue != null ? JsonConvert.DeserializeObject<T>(jsonValue) : default(T);
            return new DocumentWrapper<T>
            {
                DeserializedValue = deserializedValue,
                JsonValue = jsonValue,
                Partition = documentWrapper.Partition,
                ETag = documentWrapper.ETag,
                Id = documentWrapper.DocumentId,
                LastUpdatedDate = documentWrapper.LastUpdatedDate != null ? (DateTime)documentWrapper?.LastUpdatedDate : UnixEpoch,
                IsFromDeviceCache = documentWrapper.FromDeviceCache
            };
        }

        public static MSReadOptions ToMSReadOptions(this ReadOptions readOptions)
        {
            return new MSReadOptions
            {
                DeviceTimeToLive = (long)readOptions.DeviceTimeToLive.TotalSeconds
            };
        }

        public static MSWriteOptions ToMSWriteOptions(this WriteOptions writeOptions)
        {
            return new MSWriteOptions
            {
                DeviceTimeToLive = (long)writeOptions.DeviceTimeToLive.TotalSeconds
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
                Items = msPage.Items
                              .Select(i => i.ToDocumentWrapper<T>()).ToList()
            };
        }

        public static MSDictionaryDocument ToMSDocument<T>(this T document)
        {
            var deserialized = JsonConvert.SerializeObject(document);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(deserialized);
            var nativeDict = NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
            return new MSDictionaryDocument().Init(nativeDict);
        }

        public static T ToDocument<T>(this MSDictionaryDocument document)
        {
            var dict = document.SerializeToDictionary()
                               .ToDictionary(i => (string)(NSString)i.Key, i => (string)(NSString)i.Value);
            var serialized = JsonConvert.SerializeObject(dict);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static PaginatedDocuments<T> ToPaginatedDocuments<T>(this MSPaginatedDocuments paginatedDocuments)
        {
            return new PaginatedDocuments<T>(paginatedDocuments);
        }

        public static DocumentMetadata ToDocumentMetadata(this MSDocumentWrapper documentWrapper)
        {
            return new DocumentMetadata
            {
                Id = documentWrapper.DocumentId,
                Partition = documentWrapper.Partition,
                ETag = documentWrapper.ETag
            };
        }

        public static DataException ToDataException(this MSDataError error, MSDocumentWrapper msDocumentWrapper = null)
        {
            var exception = new NSErrorException(error);
            return new DataException(error.LocalizedDescription, exception)
            {
                DocumentMetadata = msDocumentWrapper?.ToDocumentMetadata()
            };
        }
    }
}
