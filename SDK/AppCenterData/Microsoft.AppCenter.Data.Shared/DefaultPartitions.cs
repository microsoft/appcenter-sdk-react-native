// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Constants for the default partition names.
    /// </summary>
    public static class DefaultPartitions
    {   
        /// <summary>
        /// User partition. An authenticated user can read/write documents in this partition.
        /// </summary>
        public const string UserDocuments = "user";

        /// <summary>
        /// Readonly partition. Everyone can read documents in this partition. Writes are not allowed via the SDK.
        /// </summary>
        public const string AppDocuments = "readonly";
    }
}
