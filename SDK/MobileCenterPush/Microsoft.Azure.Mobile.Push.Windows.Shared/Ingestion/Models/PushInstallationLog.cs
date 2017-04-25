using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models
{
    [JsonObject(JsonIdentifier)]
    public class PushInstallationLog : Log
    {
        internal const string JsonIdentifier = "push_installation";

        public PushInstallationLog(long toffset, Mobile.Ingestion.Models.Device device, string pushToken, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, sid)
        {
            this.PushToken = pushToken;
        }

        [JsonProperty(PropertyName = "push_token")]
        public string PushToken { get; set; }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public override void Validate()
        {
            base.Validate();
        }
    }
}
