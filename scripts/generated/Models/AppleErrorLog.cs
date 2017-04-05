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
    /// Error log for Apple platforms.
    /// </summary>
    [JsonObject("apple_error")]
    public partial class AppleErrorLog : AbstractErrorLog
    {
        /// <summary>
        /// Initializes a new instance of the AppleErrorLog class.
        /// </summary>
        public AppleErrorLog() { }

        /// <summary>
        /// Initializes a new instance of the AppleErrorLog class.
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
        /// <param name="primaryArchitectureId">CPU primary
        /// architecture.</param>
        /// <param name="applicationPath">Path to the application.</param>
        /// <param name="osExceptionType">OS exception type.</param>
        /// <param name="osExceptionCode">OS exception code.</param>
        /// <param name="osExceptionAddress">OS exception address.</param>
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
        /// <param name="architectureVariantId">CPU architecture
        /// variant.</param>
        /// <param name="exceptionType">Exception type.</param>
        /// <param name="exceptionReason">Exception reason.</param>
        /// <param name="threads">Thread stack frames associated to the
        /// error.</param>
        /// <param name="binaries">Binaries associated to the error.</param>
        /// <param name="registers">Registers.</param>
        /// <param name="exception">Exception associated to the error.
        /// This is used for example to send a .NET exception from the Xamarin
        /// SDK.
        /// </param>
        public AppleErrorLog(long toffset, Device device, System.Guid id, int processId, string processName, bool fatal, long appLaunchToffset, int primaryArchitectureId, string applicationPath, string osExceptionType, string osExceptionCode, string osExceptionAddress, System.Guid? sid = default(System.Guid?), int? parentProcessId = default(int?), string parentProcessName = default(string), long? errorThreadId = default(long?), string errorThreadName = default(string), ErrorAttachment errorAttachment = default(ErrorAttachment), string architecture = default(string), int? architectureVariantId = default(int?), string exceptionType = default(string), string exceptionReason = default(string), IList<Thread> threads = default(IList<Thread>), IList<Binary> binaries = default(IList<Binary>), IDictionary<string, string> registers = default(IDictionary<string, string>), Exception exception = default(Exception))
            : base(toffset, device, id, processId, processName, fatal, appLaunchToffset, sid, parentProcessId, parentProcessName, errorThreadId, errorThreadName, errorAttachment, architecture)
        {
            PrimaryArchitectureId = primaryArchitectureId;
            ArchitectureVariantId = architectureVariantId;
            ApplicationPath = applicationPath;
            OsExceptionType = osExceptionType;
            OsExceptionCode = osExceptionCode;
            OsExceptionAddress = osExceptionAddress;
            ExceptionType = exceptionType;
            ExceptionReason = exceptionReason;
            Threads = threads;
            Binaries = binaries;
            Registers = registers;
            Exception = exception;
        }

        /// <summary>
        /// Gets or sets CPU primary architecture.
        /// </summary>
        [JsonProperty(PropertyName = "primary_architecture_id")]
        public int PrimaryArchitectureId { get; set; }

        /// <summary>
        /// Gets or sets CPU architecture variant.
        /// </summary>
        [JsonProperty(PropertyName = "architecture_variant_id")]
        public int? ArchitectureVariantId { get; set; }

        /// <summary>
        /// Gets or sets path to the application.
        /// </summary>
        [JsonProperty(PropertyName = "application_path")]
        public string ApplicationPath { get; set; }

        /// <summary>
        /// Gets or sets OS exception type.
        /// </summary>
        [JsonProperty(PropertyName = "os_exception_type")]
        public string OsExceptionType { get; set; }

        /// <summary>
        /// Gets or sets OS exception code.
        /// </summary>
        [JsonProperty(PropertyName = "os_exception_code")]
        public string OsExceptionCode { get; set; }

        /// <summary>
        /// Gets or sets OS exception address.
        /// </summary>
        [JsonProperty(PropertyName = "os_exception_address")]
        public string OsExceptionAddress { get; set; }

        /// <summary>
        /// Gets or sets exception type.
        /// </summary>
        [JsonProperty(PropertyName = "exception_type")]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets exception reason.
        /// </summary>
        [JsonProperty(PropertyName = "exception_reason")]
        public string ExceptionReason { get; set; }

        /// <summary>
        /// Gets or sets thread stack frames associated to the error.
        /// </summary>
        [JsonProperty(PropertyName = "threads")]
        public IList<Thread> Threads { get; set; }

        /// <summary>
        /// Gets or sets binaries associated to the error.
        /// </summary>
        [JsonProperty(PropertyName = "binaries")]
        public IList<Binary> Binaries { get; set; }

        /// <summary>
        /// Gets or sets registers.
        /// </summary>
        [JsonProperty(PropertyName = "registers")]
        public IDictionary<string, string> Registers { get; set; }

        /// <summary>
        /// Gets or sets exception associated to the error.
        /// This is used for example to send a .NET exception from the Xamarin
        /// SDK.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "exception")]
        public Exception Exception { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (ApplicationPath == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "ApplicationPath");
            }
            if (OsExceptionType == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "OsExceptionType");
            }
            if (OsExceptionCode == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "OsExceptionCode");
            }
            if (OsExceptionAddress == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "OsExceptionAddress");
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
            if (Binaries != null)
            {
                foreach (var element1 in Binaries)
                {
                    if (element1 != null)
                    {
                        element1.Validate();
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

