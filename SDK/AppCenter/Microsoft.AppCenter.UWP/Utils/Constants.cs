// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Various constants used by the SDK.
    /// </summary>
    public static partial class Constants
    {
        // Local Data.
        public static readonly string LocalAppData = global::Windows.Storage.ApplicationData.Current.LocalFolder.Path;

        // File Storage.
        public static readonly string AppCenterFilesDirectoryPath = Path.Combine(LocalAppData, "Microsoft", "AppCenter");

        // The database file does not go in the main App Center files folder because it cannot be changed without migrating.
        public static readonly string AppCenterDatabasePath = "Microsoft.AppCenter.Storage";
    }
}
