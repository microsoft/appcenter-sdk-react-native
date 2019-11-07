// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models
{
    /// <summary>
    /// Exception definition for any platform.
    /// </summary>
    public partial class Exception
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public Exception()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        /// <param name="type">Exception type.</param>
        /// <param name="message">Exception reason.</param>
        /// <param name="stackTrace">Raw stack trace. Sent when the frames
        /// property is either missing or unreliable.</param>
        /// <param name="frames">Stack frames. Optional.</param>
        /// <param name="innerExceptions">Inner exceptions of this
        /// exception.</param>
        /// <param name="wrapperSdkName">Name of the wrapper SDK that emitted
        /// this exeption. Consists of the name of the SDK and the wrapper
        /// platform, e.g. "appcenter.xamarin", "hockeysdk.cordova".
        /// </param>
        public Exception(string type, string message = default(string), string stackTrace = default(string), IList<StackFrame> frames = default(IList<StackFrame>), IList<Exception> innerExceptions = default(IList<Exception>), string wrapperSdkName = default(string))
        {
            Type = type;
            Message = message;
            StackTrace = stackTrace;
            Frames = frames;
            InnerExceptions = innerExceptions;
            WrapperSdkName = wrapperSdkName;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

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
        [JsonProperty(PropertyName = "stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets stack frames. Optional.
        /// </summary>
        [JsonProperty(PropertyName = "frames")]
        public IList<StackFrame> Frames { get; set; }

        /// <summary>
        /// Gets or sets inner exceptions of this exception.
        /// </summary>
        [JsonProperty(PropertyName = "innerExceptions")]
        public IList<Exception> InnerExceptions { get; set; }

        /// <summary>
        /// Gets or sets name of the wrapper SDK that emitted this exeption.
        /// Consists of the name of the SDK and the wrapper platform, e.g.
        /// "appcenter.xamarin", "hockeysdk.cordova".
        ///
        /// </summary>
        [JsonProperty(PropertyName = "wrapperSdkName")]
        public string WrapperSdkName { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Type == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Type");
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
