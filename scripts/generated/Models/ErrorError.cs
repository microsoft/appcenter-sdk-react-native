// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ErrorError
    {
        /// <summary>
        /// Initializes a new instance of the ErrorError class.
        /// </summary>
        public ErrorError() { }

        /// <summary>
        /// Initializes a new instance of the ErrorError class.
        /// </summary>
        /// <param name="code">The status code return by the API. It can be 400
        /// or 403 or 500.</param>
        /// <param name="message">The reason for the request failed</param>
        public ErrorError(int? code = default(int?), string message = default(string))
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the status code return by the API. It can be 400 or
        /// 403 or 500.
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int? Code { get; set; }

        /// <summary>
        /// Gets or sets the reason for the request failed
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}

