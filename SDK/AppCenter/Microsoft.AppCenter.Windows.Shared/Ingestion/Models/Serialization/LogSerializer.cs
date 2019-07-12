// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models.Serialization
{
    public static class LogSerializer
    {
        internal static readonly JsonSerializerSettings SerializationSettings;
        private static readonly LogJsonConverter Converter = new LogJsonConverter();
 
        static LogSerializer()
        {
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Converters = { Converter }
            };
        }

        public static void AddLogType(string typeName, Type type)
        {
            Converter.AddLogType(typeName, type);
        }

        public static string Serialize(LogContainer logContainer)
        {
            return JsonConvert.SerializeObject(logContainer, SerializationSettings);
        }

        public static string Serialize(Log log)
        {
            return JsonConvert.SerializeObject(log, SerializationSettings);
        }

        public static Log DeserializeLog(string json)
        {
            return JsonConvert.DeserializeObject<Log>(json, SerializationSettings);
        }
    }
}
