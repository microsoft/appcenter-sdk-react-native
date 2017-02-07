// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Error
    /// </summary>
    public partial class Error
    {
        /// <summary>
        /// Initializes a new instance of the Error class.
        /// </summary>
        public Error() { }

        /// <summary>
        /// Initializes a new instance of the Error class.
        /// </summary>
        public Error(ErrorError errorProperty = default(ErrorError))
        {
            ErrorProperty = errorProperty;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public ErrorError ErrorProperty { get; set; }

    }
}

