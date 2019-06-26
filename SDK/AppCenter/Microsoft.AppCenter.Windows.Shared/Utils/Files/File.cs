// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Microsoft.AppCenter.Utils.Files
{
    public class File
    {
        private FileInfo _implementation;

        internal File(FileInfo fileInfo)
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
