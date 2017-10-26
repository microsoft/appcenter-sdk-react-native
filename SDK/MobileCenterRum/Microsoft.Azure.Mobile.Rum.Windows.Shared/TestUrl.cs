using Newtonsoft.Json;

namespace Microsoft.AppCenter.Rum
{
    internal class TestUrl
    {
        internal string Url { get; set; }

        [JsonProperty]
        internal string RequestId { get; set; }

        [JsonProperty]
        internal string Object { get; set; }

        [JsonProperty]
        internal string Conn { get; set; }

        [JsonProperty]
        internal long Result { get; set; }
    }
}
