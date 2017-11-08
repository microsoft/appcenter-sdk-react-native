// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Abstract error log.
    /// </summary>
    public partial class AbstractErrorLog : Log
    {
        /// <summary>
        /// Initializes a new instance of the AbstractErrorLog class.
        /// </summary>
        public AbstractErrorLog() { }

        /// <summary>
        /// Initializes a new instance of the AbstractErrorLog class.
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
        /// <param name="parentProcessId">Parent's process identifier.</param>
        /// <param name="parentProcessName">Parent's process name.</param>
        /// <param name="errorThreadId">Error thread identifier.</param>
        /// <param name="errorThreadName">Error thread name.</param>
        /// <param name="appLaunchTimestamp">Timestamp when the app was
        /// launched, example: '2017-03-13T18:05:42Z'.
        /// </param>
        /// <param name="architecture">CPU architecture.</param>
        public AbstractErrorLog(Device device, System.Guid id, int processId, string processName, bool fatal, System.DateTime? timestamp = default(System.DateTime?), System.Guid? sid = default(System.Guid?), int? parentProcessId = default(int?), string parentProcessName = default(string), long? errorThreadId = default(long?), string errorThreadName = default(string), System.DateTime? appLaunchTimestamp = default(System.DateTime?), string architecture = default(string))
            : base(device, timestamp, sid)
        {
            Id = id;
            ProcessId = processId;
            ProcessName = processName;
            ParentProcessId = parentProcessId;
            ParentProcessName = parentProcessName;
            ErrorThreadId = errorThreadId;
            ErrorThreadName = errorThreadName;
            Fatal = fatal;
            AppLaunchTimestamp = appLaunchTimestamp;
            Architecture = architecture;
        }

        /// <summary>
        /// Gets or sets error identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// Gets or sets process identifier.
        /// </summary>
        [JsonProperty(PropertyName = "processId")]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets process name.
        /// </summary>
        [JsonProperty(PropertyName = "processName")]
        public string ProcessName { get; set; }

        /// <summary>
        /// Gets or sets parent's process identifier.
        /// </summary>
        [JsonProperty(PropertyName = "parentProcessId")]
        public int? ParentProcessId { get; set; }

        /// <summary>
        /// Gets or sets parent's process name.
        /// </summary>
        [JsonProperty(PropertyName = "parentProcessName")]
        public string ParentProcessName { get; set; }

        /// <summary>
        /// Gets or sets error thread identifier.
        /// </summary>
        [JsonProperty(PropertyName = "errorThreadId")]
        public long? ErrorThreadId { get; set; }

        /// <summary>
        /// Gets or sets error thread name.
        /// </summary>
        [JsonProperty(PropertyName = "errorThreadName")]
        public string ErrorThreadName { get; set; }

        /// <summary>
        /// Gets or sets if true, this error report is an application crash.
        /// Corresponds to the number of milliseconds elapsed between the time
        /// the error occurred and the app was launched.
        /// </summary>
        [JsonProperty(PropertyName = "fatal")]
        public bool Fatal { get; set; }

        /// <summary>
        /// Gets or sets timestamp when the app was launched, example:
        /// '2017-03-13T18:05:42Z'.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "appLaunchTimestamp")]
        public System.DateTime? AppLaunchTimestamp { get; set; }

        /// <summary>
        /// Gets or sets CPU architecture.
        /// </summary>
        [JsonProperty(PropertyName = "architecture")]
        public string Architecture { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (ProcessName == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "ProcessName");
            }
        }
    }
}

