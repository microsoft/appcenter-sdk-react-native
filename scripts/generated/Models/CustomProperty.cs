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

    public partial class CustomProperty
    {
        /// <summary>
        /// Initializes a new instance of the CustomProperty class.
        /// </summary>
        public CustomProperty() { }

        /// <summary>
        /// Initializes a new instance of the CustomProperty class.
        /// </summary>
        public CustomProperty(string name)
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Name");
            }
            if (Name != null)
            {
                if (Name.Length > 128)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MaxLength, "Name", 128);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(Name, "^[a-zA-Z][a-zA-Z0-9\\-_]*$"))
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.Pattern, "Name", "^[a-zA-Z][a-zA-Z0-9\\-_]*$");
                }
            }
        }
    }
}

