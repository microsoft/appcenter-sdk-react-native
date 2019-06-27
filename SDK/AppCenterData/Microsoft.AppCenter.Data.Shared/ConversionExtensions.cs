using System;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Data
{
    public static class SharedConversionExtensions
    {
        public static DocumentWrapper<T> ToDocumentWrapper<T>(
            string partition,
            string eTag,
            string id,
            bool isFromDeviceCache,
            string jsonValue,
            DateTimeOffset date)
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
                    parsedDocument.DeserializedValue = JsonConvert.DeserializeObject<T>(parsedDocument.JsonValue);
                }
            }
            catch (Exception ex)
            {
                parsedDocument.Error = new DataException(ex.Message);
            }
            return parsedDocument;
        }
    }
}
