using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Rum
{
    internal class TestEndpoint
    {
        [JsonProperty("e")]
        internal string Url { get; set; }

        [JsonProperty("m")]
        internal int MeasurementType { get; set; }

        [JsonProperty("w")]
        internal int Weight { get; set; }

        internal int CumulatedWeight { get; set; }
    }
}
