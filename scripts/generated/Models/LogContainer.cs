// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.UWP;
    using Microsoft.AppCenter.UWP.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class LogContainer
    {
        /// <summary>
        /// Initializes a new instance of the LogContainer class.
        /// </summary>
        public LogContainer() { }

        /// <summary>
        /// Initializes a new instance of the LogContainer class.
        /// </summary>
        /// <param name="logs">The list of logs</param>
        public LogContainer(IList<Log> logs)
        {
            Logs = logs;
        }

        /// <summary>
        /// Gets or sets the list of logs
        /// </summary>
        [JsonProperty(PropertyName = "logs")]
        public IList<Log> Logs { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Logs == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Logs");
            }
            if (Logs != null)
            {
                if (Logs.Count < 1)
                {
                    throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.MinItems, "Logs", 1);
                }
                foreach (var element in Logs)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}

