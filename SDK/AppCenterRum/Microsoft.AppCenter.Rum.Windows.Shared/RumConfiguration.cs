// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Rum
{
    internal class RumConfiguration
    {
        [JsonProperty("n")]
        internal int TestCount { get; set; }

        [JsonProperty("e")]
        internal IList<TestEndpoint> TestEndpoints { get; set; }

        [JsonProperty("r")]
        internal IList<string> ReportEndpoints { get; set; }
    }
}
