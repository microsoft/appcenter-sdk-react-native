// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models
{
    /// <summary>
    /// Error log for managed platforms (such as Android Dalvik/ART).
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public partial class ManagedErrorLog : AbstractErrorLog
    {
        internal const string JsonIdentifier = "managedError";

        /// <summary>
        /// Initializes a new instance of the ManagedErrorLog class.
        /// </summary>
        public ManagedErrorLog()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ManagedErrorLog class.
        /// </summary>
        /// <param name="id">Error identifier.</param>
        /// <param name="processId">Process identifier.</param>
        /// <param name="processName">Process name.</param>
        /// <param name="fatal">If true, this error report is an application
        /// crash.
        /// Corresponds to the number of milliseconds elapsed between the time
        /// the error occurred and the app was launched.</param>
        /// <param name="timestamp">Log timestamp, example:
        /// '2017-03-13T18:05:42Z'.
        /// </param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        /// <param name="userId">optional string used for associating logs with
        /// users.
        /// </param>
        /// <param name="parentProcessId">Parent's process identifier.</param>
        /// <param name="parentProcessName">Parent's process name.</param>
        /// <param name="errorThreadId">Error thread identifier.</param>
        /// <param name="errorThreadName">Error thread name.</param>
        /// <param name="appLaunchTimestamp">Timestamp when the app was
        /// launched, example: '2017-03-13T18:05:42Z'.
        /// </param>
        /// <param name="architecture">CPU architecture.</param>
        /// <param name="buildId">Unique ID for a Xamarin build or another
        /// similar technology.</param>
        public ManagedErrorLog(Microsoft.AppCenter.Ingestion.Models.Device device, System.Guid id, int processId, string processName, bool fatal, Exception exception, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), string userId = default(string), int? parentProcessId = default(int?), string parentProcessName = default(string), long? errorThreadId = default(long?), string errorThreadName = default(string), System.DateTime? appLaunchTimestamp = default(System.DateTime?), string architecture = default(string), IList<Binary> binaries = default(IList<Binary>), string buildId = default(string))
            : base(device, id, processId, processName, fatal, timestamp, sid, userId, parentProcessId, parentProcessName, errorThreadId, errorThreadName, appLaunchTimestamp, architecture)
        {
            Binaries = binaries;
            BuildId = buildId;
            Exception = exception;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "binaries")]
        public IList<Binary> Binaries { get; set; }

        /// <summary>
        /// Gets or sets unique ID for a Xamarin build or another similar
        /// technology.
        /// </summary>
        [JsonProperty(PropertyName = "buildId")]
        public string BuildId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "exception")]
        public Exception Exception { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Exception == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Exception");
            }
            if (Binaries != null)
            {
                foreach (var element in Binaries)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (Exception != null)
            {
                Exception.Validate();
            }
        }
    }
}
