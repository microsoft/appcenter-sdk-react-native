// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Exception thrown by asynchronous Data service APIs.
    /// </summary>
    public class DataException : Exception
    {
        /// <summary>
        /// Optional document metadata.
        /// </summary>
        public DocumentMetadata DocumentMetadata { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.DataException" /> class.
        /// </summary>
        public DataException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.DataException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public DataException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.DataException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified. </param>
        public DataException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
