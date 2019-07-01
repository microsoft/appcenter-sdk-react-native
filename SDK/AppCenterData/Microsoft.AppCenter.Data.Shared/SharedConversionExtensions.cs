// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    public static class SharedConversionExtensions
    {
        public static DocumentWrapper<T> ToDocumentWrapper<T>(string partition, string eTag, string id, bool isFromDeviceCache,
            string jsonValue, Func<string, T> deserealizer, DateTimeOffset date)
        {
            DocumentWrapper<T> parsedDocument = new DocumentWrapper<T>
            {
                Partition = partition,
                ETag = eTag,
                Id = id,
                IsFromDeviceCache = isFromDeviceCache,
                JsonValue = jsonValue,
                DeserializedValue = default(T),
                LastUpdatedDate = date
            };

            // Try to deserialize.
            try
            {
                // Set deserialized value.
                if (parsedDocument.JsonValue != null)
                {
                    parsedDocument.DeserializedValue = deserealizer(parsedDocument.JsonValue);
                }
            }
            catch (Exception ex)
            {
                parsedDocument.Error =
                    new DataException(ex.Message)
                    {
                        DocumentMetadata = new DocumentMetadata
                        {
                            Partition = partition,
                            Id = id,
                            ETag = eTag
                        }
                    };
            }
            return parsedDocument;
        }
    }
}
