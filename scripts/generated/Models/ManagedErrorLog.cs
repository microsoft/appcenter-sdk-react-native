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

    /// <summary>
    /// Error log for managed platforms (such as Android Dalvik/ART).
    /// </summary>
    [JsonObject("managed_error")]
    public partial class ManagedErrorLog : AbstractErrorLog
    {
        /// <summary>
        /// Initializes a new instance of the ManagedErrorLog class.
        /// </summary>
        public ManagedErrorLog() { }

        /// <summary>
        /// Initializes a new instance of the ManagedErrorLog class.
        /// </summary>
        /// <param name="toffset">Corresponds to the number of milliseconds
        /// elapsed between the time the request is sent and the time the log
        /// is emitted.</param>
        /// <param name="id">Error identifier.</param>
        /// <param name="processId">Process identifier.</param>
        /// <param name="processName">Process name.</param>
        /// <param name="fatal">If true, this error report is an application
        /// crash.</param>
        /// <param name="appLaunchToffset">Corresponds to the number of
        /// milliseconds elapsed between the time the error occurred and the
        /// app was launched.</param>
        /// <param name="exception">Exception associated to the error.</param>
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
        /// <param name="errorAttachment">Error attachment.</param>
        /// <param name="architecture">CPU architecture.</param>
        /// <param name="buildId">Unique ID for a Xamarin build or another
        /// similar technology.</param>
        /// <param name="threads">Thread stack frames associated to the
        /// error.</param>
        public ManagedErrorLog(long toffset, Device device, System.Guid id, int processId, string processName, bool fatal, long appLaunchToffset, Exception exception, System.Guid? sid = default(System.Guid?), int? parentProcessId = default(int?), string parentProcessName = default(string), long? errorThreadId = default(long?), string errorThreadName = default(string), ErrorAttachment errorAttachment = default(ErrorAttachment), string architecture = default(string), string buildId = default(string), IList<Thread> threads = default(IList<Thread>))
            : base(toffset, device, id, processId, processName, fatal, appLaunchToffset, sid, parentProcessId, parentProcessName, errorThreadId, errorThreadName, errorAttachment, architecture)
        {
            BuildId = buildId;
            Exception = exception;
            Threads = threads;
        }

        /// <summary>
        /// Gets or sets unique ID for a Xamarin build or another similar
        /// technology.
        /// </summary>
        [JsonProperty(PropertyName = "build_id")]
        public string BuildId { get; set; }

        /// <summary>
        /// Gets or sets exception associated to the error.
        /// </summary>
        [JsonProperty(PropertyName = "exception")]
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets thread stack frames associated to the error.
        /// </summary>
        [JsonProperty(PropertyName = "threads")]
        public IList<Thread> Threads { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Exception == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Exception");
            }
            if (Exception != null)
            {
                Exception.Validate();
            }
            if (Threads != null)
            {
                foreach (var element in Threads)
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

