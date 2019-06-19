// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Utilities for the file system.
    /// </summary>
    public partial class FileHelper
    {
        /// <summary>
        /// Path to the directory where App Center files are stored.
        /// </summary>
        public static string AppCenterFolderPath => Path.Combine(AppCenterFolderLocation, AppCenterFolderName);

        private static string AppCenterFolderName = "Microsoft.AppCenter";

        private readonly DirectoryInfo _baseDirectory;

        /// <summary>
        /// Creates an instance. This parameterless constructor is needed for tests.
        /// </summary>
        public FileHelper()
        {
            _baseDirectory = new DirectoryInfo(AppCenterFolderPath);
        }

        /// <summary>
        /// Creates an instance with the given base path.
        /// </summary>
        /// <param name="baseDirectoryName">The path to use as the base for all operations.</param>
        public FileHelper(string baseDirectoryName)
        {
            baseDirectoryName = Path.Combine(AppCenterFolderPath, baseDirectoryName);
            _baseDirectory = new DirectoryInfo(baseDirectoryName);
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
        public virtual void CreateFile(string relativePath, string contents)
        {
            if (!_baseDirectory.Exists)
            {
                _baseDirectory.Create();
            }
            var filePath = Path.Combine(_baseDirectory.FullName, relativePath);
            File.WriteAllText(filePath, contents);
        }
    }
}