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
            List<MSDocumentWrapper> lst = msPage.Items.OfType<MSDocumentWrapper>().ToList();
            if (msPage.Error != null)
            {
                throw msPage.Error.ToDataException();
            }
            return new Page<T>
            {
                Items = lst
                    .Cast<MSDocumentWrapper>()
                    .Select(i => i.ToDocumentWrapper<T>()).ToList()
            };
        }

        public static MSDictionaryDocument ToMSDocument<T>(this T document)
        {
            NSDictionary dic = JsonConvert.DeserializeObject<NSDictionary>(JsonConvert.SerializeObject(document));
            return new MSDictionaryDocument.Init(dic);
        }

        public static DataException ToDataException(this MSDataError error) 
        {
            var exception = new NSErrorException(error.Error);
            return new DataException(error.Error.LocalizedDescription, exception);
        }
    }
}
