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
        private readonly DirectoryInfo _implementation;

        /// <summary>
        /// Parameterless constructor needed for testing.
        /// </summary>
        public Directory()
        {
        }

        public Directory(string directoryPath)
        {
            _implementation = new DirectoryInfo(directoryPath);
        }

        public virtual string FullName => _implementation.FullName;

        public virtual IEnumerable<File> EnumerateFiles(string searchPattern)
        {
            return _implementation.EnumerateFiles(searchPattern).Select(fileInfo => new File(fileInfo));
        }

        public virtual void CreateFile(string name, string contents)
        {
            var filePath = System.IO.Path.Combine(FullName, name);
            System.IO.File.WriteAllText(filePath, contents);
        }

        public virtual void Create()
        {
            _implementation.Create();
        }

        public virtual void Delete(bool recursive)
        {
            _implementation.Delete(recursive);
        }
    }
}
