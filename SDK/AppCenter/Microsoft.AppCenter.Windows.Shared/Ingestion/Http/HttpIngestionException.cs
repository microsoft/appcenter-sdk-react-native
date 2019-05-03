// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class HttpIngestionException : IngestionException
    {
        public string Method { get; set; }
        public Uri RequestUri { get; set; }
        public string RequestContent { get; set; }
        public int StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public override bool IsRecoverable
        {
            get
            {
                var statusCode = (int)StatusCode;
                return statusCode >= 500 || statusCode == 408;
            }
        }

        public HttpIngestionException(string message) : base(message)
        {
        }
    }
}
