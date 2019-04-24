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
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
                FromDeviceCache = documentWrapper.FromDeviceCache
            };
        }

        public static MSReadOptions ToiOSReadOptions(this ReadOptions readOptions)
        {
            return new MSReadOptions
            {
                DeviceTimeToLive = readOptions.DeviceTimeToLive.Ticks
            };
        }

        public static MSWriteOptions ToiOSWriteOptions(this WriteOptions writeOptions)
        {
            return new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };
        }
    }
}
