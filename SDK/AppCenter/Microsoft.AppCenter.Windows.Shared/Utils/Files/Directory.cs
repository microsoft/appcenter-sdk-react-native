// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.AppCenter.Utils.Files
{
    /// <summary>
    /// This class wraps System.IO.DirectoryInfo for the unit tests.
    /// </summary>
    public class Directory
    {
        private readonly DirectoryInfo _underlyingDirectoryInfo;

        /// <summary>
        /// Parameterless constructor needed for testing.
        /// </summary>
        public Directory()
        {
        }

        public Directory(string directoryPath)
        {
            _underlyingDirectoryInfo = new DirectoryInfo(directoryPath);
        }

        public virtual string FullName => _underlyingDirectoryInfo.FullName;

        public virtual IEnumerable<File> EnumerateFiles(string searchPattern)
        {
            return _underlyingDirectoryInfo.EnumerateFiles(searchPattern).Select(fileInfo => new File(fileInfo));
        }

        public virtual void CreateFile(string name, string contents)
        {
            var filePath = Path.Combine(FullName, name);
            System.IO.File.WriteAllText(filePath, contents);
        }

        public virtual void Create()
        {
            _underlyingDirectoryInfo.Create();
        }

        public virtual void Delete(bool recursive)
        {
            _underlyingDirectoryInfo.Delete(recursive);
        }

        public virtual bool Exists()
        {
            return _underlyingDirectoryInfo.Exists;
        }

        public virtual void Refresh()
        {
            _underlyingDirectoryInfo.Refresh();
        }
    }
}
