// Copyright (c) Microsoft Corporation.  All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Ingestion.Models
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
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Logs == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(Logs));
            }
            if (Logs.Count == 0)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeEmpty, nameof(Logs));
            }
            foreach (var element in Logs)
            {
                element?.Validate();
            }
        }
    }
}
