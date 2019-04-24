// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Com.Microsoft.Appcenter.Data.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Data
{
    public static class ConversionExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DocumentWrapper<T> ToDocumentWrapper<T>(this AndroidDocumentWrapper documentWrapper)
        {
            var deserializedValue = JsonConvert.DeserializeObject<T>(documentWrapper.JsonValue);
            var lastUpdateDate = UnixEpoch.AddTicks(documentWrapper.LastUpdatedDate * TimeSpan.TicksPerSecond);
            return new DocumentWrapper<T>
            {
                DeserializedValue = deserializedValue,
                Partition = documentWrapper.Partition,
                Id = documentWrapper.Id,
                ETag = documentWrapper.ETag,
                LastUpdatedDate = lastUpdateDate,
                FromDeviceCache = documentWrapper.IsFromDeviceCache,
                PendingOperation = documentWrapper.PendingOperation,
                Error = new DataException("" /* TODO */, documentWrapper.Error)
            };
        }

        public static Page<T> ToPage<T>(this AndroidPage page)
        {
            return new Page<T>(); // TODO
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
