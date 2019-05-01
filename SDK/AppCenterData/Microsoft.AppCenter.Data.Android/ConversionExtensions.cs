// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Com.Microsoft.Appcenter.Data.Exception;
using Com.Microsoft.Appcenter.Data.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Data
{
    public static class ConversionExtensions
    {
        public static DocumentMetadata ToDocumentMetadata(this AndroidDocumentMetadata documentMetadata)
        {
            return new DocumentMetadata
            {
                Partition = documentMetadata.Partition,
                Id = documentMetadata.Id,
                ETag = documentMetadata.ETag
            };
        }

        public static DataException ToDataException(this AndroidDataException error, AndroidDocumentMetadata documentMetadata = null)
        {
            return new DataException(error.Message, error)
            {
                DocumentMetadata = documentMetadata?.ToDocumentMetadata()
            };
        }

        public static DocumentWrapper<T> ToDocumentWrapper<T>(this AndroidDocumentWrapper documentWrapper)
        {
            if (documentWrapper.Error != null)
            {
                throw documentWrapper.Error.ToDataException(documentWrapper);
            }
            var jsonValue = documentWrapper.JsonValue;
            var deserializedValue = JsonConvert.DeserializeObject<T>(jsonValue);
            var lastUpdateDate = DateTimeOffset.FromUnixTimeMilliseconds(documentWrapper.LastUpdatedDate.Time);
            return new DocumentWrapper<T>
            {
                DeserializedValue = deserializedValue,
                JsonValue = jsonValue,
                Partition = documentWrapper.Partition,
                Id = documentWrapper.Id,
                ETag = documentWrapper.ETag,
                LastUpdatedDate = lastUpdateDate,
                IsFromDeviceCache = documentWrapper.IsFromDeviceCache
            };
        }

        public static PaginatedDocuments<T> ToPaginatedDocuments<T>(this AndroidPaginatedDocuments paginatedDocuments)
        {
            // If first page is in error, don't wait until iteration to throw.
            if (paginatedDocuments.CurrentPage.Error != null)
            {
                throw paginatedDocuments.CurrentPage.Error.ToDataException();
            }
            return new PaginatedDocuments<T>(paginatedDocuments);
        }

        public static Page<T> ToPage<T>(this AndroidPage page)
        {
            if (page.Error != null)
            {
                throw page.Error.ToDataException();
            }
            return new Page<T>
            {
                Items = page.Items
                    .Cast<AndroidDocumentWrapper>()
                    .Select(i => i.ToDocumentWrapper<T>()).ToList()
            };
        }

        public static AndroidReadOptions ToAndroidReadOptions(this ReadOptions readOptions)
        {
            return new AndroidReadOptions((int)readOptions.DeviceTimeToLive.TotalSeconds);
        }

        public static AndroidWriteOptions ToAndroidWriteOptions(this WriteOptions writeOptions)
        {
            return new AndroidWriteOptions((int)writeOptions.DeviceTimeToLive.TotalSeconds);
        }
    }
}
