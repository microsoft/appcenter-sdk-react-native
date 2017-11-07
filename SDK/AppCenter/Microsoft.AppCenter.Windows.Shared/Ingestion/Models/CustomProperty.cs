// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class CustomProperty
    {
        private const int MaxNameLength = 128;
        private const string KeyPattern = "^[a-zA-Z][a-zA-Z0-9\\-_]*$";

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
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(Name));
            }
            if (Name != null)
            {
                if (Name.Length > MaxNameLength)
                {
                    throw new ValidationException(ValidationException.Rule.MaxLength, nameof(Name), MaxNameLength);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(Name, KeyPattern))
                {
                    throw new ValidationException(ValidationException.Rule.Pattern, nameof(Name), KeyPattern);
                }
            }
        }
    }
}

