// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Utilities for the file system.
    /// </summary>
    public partial class FileHelper
    {
        private static string AppCenterFolderLocation => global::Windows.Storage.ApplicationData.Current.LocalFolder.Path;
    }
}