// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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
            if (Logs != null)
            {
                if (Logs.Count < 1)
                {
                    throw new ValidationException(ValidationException.Rule.MinItems, nameof(Logs), 1);
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
