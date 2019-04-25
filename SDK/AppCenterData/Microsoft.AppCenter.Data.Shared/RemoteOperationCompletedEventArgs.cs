// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Remote operation completed event arguments.
    /// </summary>
    public class RemoteOperationCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// The remote operation.
        /// </summary>
        /// <value>The operation.</value>
        public string Operation { get; internal set; }

        /// <summary>
        /// The document metadata.
        /// </summary>
        /// <value>The document metadata.</value>
        public DocumentMetadata DocumentMetadata { get; internal set; }

        /// <summary>
        /// The document error, or null if the operation was successful.
        /// </summary>
        /// <value>The error.</value>
        public DataException Error { get; internal set; }
    }
}
