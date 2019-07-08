// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils.Files;
using System;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public partial class ErrorLogHelper
    {
        private void SaveExceptionFile(Directory directory, string fileName, Exception exception)
        {
            // We don't support serializing exception for client side inspection on UWP.
        }

        /// <summary>
        /// Reads an exception file from the given file.
        /// </summary>
        /// <param name="file">The file that contains exception.</param>
        /// <returns>An exception instance or null if the file doesn't contain an exception.</returns>
        public virtual Exception InstanceReadExceptionFile(File file)
        {
            // Not Supported in UWP.
            return null;
        }
    }
}
