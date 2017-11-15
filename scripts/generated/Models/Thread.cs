// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Thread definition for any platform.
    /// </summary>
    public partial class Thread
    {
        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        public Thread() { }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="id">Thread identifier.</param>
        /// <param name="frames">Stack frames.</param>
        /// <param name="name">Thread name.</param>
        public Thread(int id, IList<StackFrame> frames, string name = default(string), Exception exception = default(Exception))
        {
            Id = id;
            Name = name;
            Frames = frames;
            Exception = exception;
        }

        /// <summary>
        /// Gets or sets thread identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets thread name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets stack frames.
        /// </summary>
        [JsonProperty(PropertyName = "frames")]
        public IList<StackFrame> Frames { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "exception")]
        public Exception Exception { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Frames == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Frames");
            }
            if (Exception != null)
            {
                Exception.Validate();
            }
        }
    }
}

