// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Microsoft.Azure.Mobile.UWP.Ingestion;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Stack frame definition for any platform.
    /// </summary>
    public partial class StackFrame
    {
        /// <summary>
        /// Initializes a new instance of the StackFrame class.
        /// </summary>
        public StackFrame() { }

        /// <summary>
        /// Initializes a new instance of the StackFrame class.
        /// </summary>
        /// <param name="address">Frame address.</param>
        /// <param name="code">Symbolized code line</param>
        /// <param name="className">The fully qualified name of the Class
        /// containing the execution point represented by this stack trace
        /// element.</param>
        /// <param name="methodName">The name of the method containing the
        /// execution point represented by this stack trace element.</param>
        /// <param name="lineNumber">The line number of the source line
        /// containing the execution point represented by this stack trace
        /// element.</param>
        /// <param name="fileName">The name of the file containing the
        /// execution point represented by this stack trace element.</param>
        public StackFrame(string address = default(string), string code = default(string), string className = default(string), string methodName = default(string), int? lineNumber = default(int?), string fileName = default(string))
        {
            Address = address;
            Code = code;
            ClassName = className;
            MethodName = methodName;
            LineNumber = lineNumber;
            FileName = fileName;
        }

        /// <summary>
        /// Gets or sets frame address.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets symbolized code line
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified name of the Class containing the
        /// execution point represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "class_name")]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the name of the method containing the execution point
        /// represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "method_name")]
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the line number of the source line containing the
        /// execution point represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "line_number")]
        public int? LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the file containing the execution point
        /// represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

    }
}

