// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Exception definition for any platform.
    /// </summary>
    public partial class Exception
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public Exception() { }

        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        /// <param name="type">Exception type.</param>
        /// <param name="frames">Stack frames.</param>
        /// <param name="message">Exception reason.</param>
        /// <param name="stackTrace">Raw stack trace. Sent when the frames
        /// property is either missing or unreliable.</param>
        /// <param name="innerExceptions">Inner exceptions of this
        /// exception.</param>
        /// <param name="wrapperSdkName">Name of the wrapper SDK that emitted
        /// this exeption. Consists of the name of the SDK and the wrapper
        /// platform, e.g. "mobilecenter.xamarin", "hockeysdk.cordova".
        /// </param>
        public Exception(string type, IList<StackFrame> frames, string message = default(string), string stackTrace = default(string), IList<Exception> innerExceptions = default(IList<Exception>), string wrapperSdkName = default(string))
        {
            Type = type;
            Message = message;
            StackTrace = stackTrace;
            Frames = frames;
            InnerExceptions = innerExceptions;
            WrapperSdkName = wrapperSdkName;
        }

        /// <summary>
        /// Gets or sets exception type.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets exception reason.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets raw stack trace. Sent when the frames property is
        /// either missing or unreliable.
        /// </summary>
        [JsonProperty(PropertyName = "stack_trace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets stack frames.
        /// </summary>
        [JsonProperty(PropertyName = "frames")]
        public IList<StackFrame> Frames { get; set; }

        /// <summary>
        /// Gets or sets inner exceptions of this exception.
        /// </summary>
        [JsonProperty(PropertyName = "inner_exceptions")]
        public IList<Exception> InnerExceptions { get; set; }

        /// <summary>
        /// Gets or sets name of the wrapper SDK that emitted this exeption.
        /// Consists of the name of the SDK and the wrapper platform, e.g.
        /// "mobilecenter.xamarin", "hockeysdk.cordova".
        ///
        /// </summary>
        [JsonProperty(PropertyName = "wrapper_sdk_name")]
        public string WrapperSdkName { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Type == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Type");
            }
            if (Frames == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Frames");
            }
            if (InnerExceptions != null)
            {
                foreach (var element in InnerExceptions)
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

