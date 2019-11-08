// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models
{
    /// <summary>
    /// Stack frame definition for any platform.
    /// </summary>
    public partial class StackFrame
    {
        /// <summary>
        /// Initializes a new instance of the StackFrame class.
        /// </summary>
        public StackFrame()
        {
            CustomInit();
        }

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
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

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
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the name of the method containing the execution point
        /// represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "methodName")]
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the line number of the source line containing the
        /// execution point represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "lineNumber")]
        public int? LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the file containing the execution point
        /// represented by this stack trace element.
        /// </summary>
        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }
    }
}
