// Copyright (c) Microsoft Corporation.  All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public class LogContainer
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
                throw new Rest.ValidationException(Rest.ValidationRules.CannotBeNull, "Logs");
            }
            if (Logs.Count < 1)
            {
                throw new Rest.ValidationException(Rest.ValidationRules.MinItems, "Logs", 1);
            }
            foreach (var element in Logs)
            {
                element?.Validate();
            }
        }
    }
}
