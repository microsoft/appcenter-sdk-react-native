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
    /// Device mapping
    /// </summary>
    public partial class DeviceMapping
    {
        /// <summary>
        /// Initializes a new instance of the DeviceMapping class.
        /// </summary>
        public DeviceMapping() { }

        /// <summary>
        /// Initializes a new instance of the DeviceMapping class.
        /// </summary>
        /// <param name="os">Operating system. Possible values include:
        /// 'Unknown', 'iOS', 'Android'</param>
        /// <param name="name">Device name</param>
        /// <param name="man">Manufacturer name</param>
        public DeviceMapping(string os = default(string), string name = default(string), string man = default(string))
        {
            Os = os;
            Name = name;
            Man = man;
        }

        /// <summary>
        /// Gets or sets operating system. Possible values include: 'Unknown',
        /// 'iOS', 'Android'
        /// </summary>
        [JsonProperty(PropertyName = "os")]
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets device name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets manufacturer name
        /// </summary>
        [JsonProperty(PropertyName = "man")]
        public string Man { get; set; }

    }
}

