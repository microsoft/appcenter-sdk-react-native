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
        // File Storage.
        public static readonly string AppCenterFilesDirectoryPath = Path.Combine(global::Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Microsoft", "AppCenter");
    }
}
