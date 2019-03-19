// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Binary (library) definition for any platform.
    /// </summary>
    public partial class Binary
    {
        /// <summary>
        /// Initializes a new instance of the Binary class.
        /// </summary>
        public Binary() { }

        /// <summary>
        /// Initializes a new instance of the Binary class.
        /// </summary>
        /// <param name="primaryArchitectureId">CPU primary
        /// architecture.</param>
        /// <param name="architectureVariantId">CPU architecture
        /// variant.</param>
        public Binary(System.Guid id, string startAddress, string endAddress, string name, string path, string architecture = default(string), long? primaryArchitectureId = default(long?), long? architectureVariantId = default(long?))
        {
            Id = id;
            StartAddress = startAddress;
            EndAddress = endAddress;
            Name = name;
            Path = path;
            Architecture = architecture;
            PrimaryArchitectureId = primaryArchitectureId;
            ArchitectureVariantId = architectureVariantId;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "startAddress")]
        public string StartAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "endAddress")]
        public string EndAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "architecture")]
        public string Architecture { get; set; }

        /// <summary>
        /// Gets or sets CPU primary architecture.
        /// </summary>
        [JsonProperty(PropertyName = "primaryArchitectureId")]
        public long? PrimaryArchitectureId { get; set; }

        /// <summary>
        /// Gets or sets CPU architecture variant.
        /// </summary>
        [JsonProperty(PropertyName = "architectureVariantId")]
        public long? ArchitectureVariantId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (StartAddress == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "StartAddress");
            }
            if (EndAddress == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "EndAddress");
            }
            if (Name == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Name");
            }
            if (Path == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Path");
            }
        }
    }
}

