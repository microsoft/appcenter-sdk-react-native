// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils.Files
{
    /// <summary>
    /// This class wraps FileInfo for the 
    /// </summary>
    public class File
    {
        private readonly System.IO.FileInfo _implementation;

        /// <summary>
        /// Parameterless constructor needed for testing.
        /// </summary>
        public File()
        {
        }

        internal File(System.IO.FileInfo fileInfo)
        {
            _implementation = fileInfo;
        }

        public virtual DateTime LastWriteTime => _implementation.LastWriteTime;

        public virtual string Name => _implementation.Name;

        public virtual void Delete()
        {
            _implementation.Delete();
        }
    }
}
