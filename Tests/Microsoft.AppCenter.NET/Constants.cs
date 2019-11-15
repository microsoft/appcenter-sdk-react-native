// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Various constants used by the SDK.
    /// </summary>
    public static partial class Constants
    {
        public static string AppCenterFilesDirectoryPath = "";

        public static string AppCenterDatabasePath = "Microsoft.AppCenter.Storage";

        public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
