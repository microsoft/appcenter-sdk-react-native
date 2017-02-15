using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Mobile.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Test
{
    [JsonObject(JsonIdentifier)]
    public class TestLog : LogWithProperties
    {
        internal const string JsonIdentifier = "testlog";
    }
}
