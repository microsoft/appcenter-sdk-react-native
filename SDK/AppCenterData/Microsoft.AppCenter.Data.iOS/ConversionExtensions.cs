// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using System;
using Newtonsoft.Json;
using Microsoft.AppCenter.Data.iOS.Bindings;

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
            var page = new Page<T>();
            foreach (var item in msPage.Items)
            {
                page.Items.Add(item.ToDocumentWrapper<T>());
            }
            return page;
        }

        public static MSSerializableDocument ToMSDocument<T>(this T document)
        {
            NSDictionary dic = JsonConvert.DeserializeObject<NSDictionary>(JsonConvert.SerializeObject(document));
            return new MSSerializableDocument().init(dic);
        }
    }
}
