using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Rum
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
