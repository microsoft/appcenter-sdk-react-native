// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Various constants used by the SDK.
    /// </summary>
    public static partial class Constants
    {
        // File Storage.
        // These aren't exactly "Constants," per se, but probably not worth moving somewhere else.
        private static string AppCenterFilesDirectoryPathBacking;

        public static string AppCenterFilesDirectoryPath
        {
            get
            {
                if (AppCenterFilesDirectoryPathBacking == null)
                {
                    // This shouldn't block in reality.
                    var installId = AppCenter.GetInstallIdAsync().Result.ToString();
                    AppCenterFilesDirectoryPathBacking = Path.Combine(LocalAppData, "Microsoft", "AppCenter", installId);
                }
                return AppCenterFilesDirectoryPathBacking;
            }
        }

        public static string AppCenterDatabasePath = Path.Combine(AppCenterFilesDirectoryPath, "Logs.db");

        public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
