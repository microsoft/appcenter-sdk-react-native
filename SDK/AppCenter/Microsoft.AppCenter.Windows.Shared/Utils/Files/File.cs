// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Microsoft.AppCenter.Utils.Files
{
    /// <summary>
    /// This class wraps System.IO.FileInfo for the unit tests.
    /// </summary>
    public class File
    {
        private readonly FileInfo _underlyingFileInfo;

        /// <summary>
        /// Parameterless constructor needed for testing.
        /// </summary>
        public File()
        {
        }

        internal File(FileInfo fileInfo)
        {
            _underlyingFileInfo = fileInfo;
        }

        public virtual string Name => _underlyingFileInfo.Name;

        public virtual string FullName => _underlyingFileInfo.FullName;

        public virtual void Delete()
        {
            _underlyingFileInfo.Delete();
        }

        public virtual string ReadAllText()
        {
            return System.IO.File.ReadAllText(FullName);
        }
    }
}
