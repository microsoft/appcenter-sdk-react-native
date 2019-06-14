// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Utilities for the file system.
    /// </summary>
    public class FileHelper
    {
        private DirectoryInfo _baseDirectory;

        public FileHelper(string basePath)
        {
            _baseDirectory = new DirectoryInfo(basePath);
        }

        /// <summary>
        /// Enumerates files with the given pattern relative to the base path.
        /// </summary>
        /// <param name="pattern">The search path to match. (can use '*' wildcards).</param>
        /// <returns>An enumerator for the files.</returns>
        public virtual IEnumerable<FileInfo> EnumerateFiles(string pattern)
        {
            return _baseDirectory.EnumerateFiles(pattern);
        }

        /// <summary>
        /// Creates or replaces a file at the given relative path with the provided contents.
        /// </summary>
        /// <param name="relativePath">The destination path relative to the base path.</param>
        /// <param name="contents">The text to write to the file.</param>
        public virtual void WriteFileContents(string relativePath, string contents)
        {
            if (!_baseDirectory.Exists)
            {
                _baseDirectory.Create();
            }
            var filePath = Path.Combine(_baseDirectory.FullName, relativePath);
            File.WriteAllText(filePath, contents);
        }

        /// <summary>
        /// Deletes a file at the given relative path.
        /// </summary>
        /// <param name="relativePath">The destination path relative to the base path.</param>
        public virtual void DeleteFile(string relativePath)
        {
            if (!_baseDirectory.Exists)
            {
                return;
            }
            var filePath = Path.Combine(_baseDirectory.FullName, relativePath);
            File.Delete(filePath);
        }
    }
}