// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Crashes.Windows.Shared.Utils
{
    /// <summary>
    /// MemoryWarningHelper to help listen to increased memory.
    /// </summary>
    public interface IMemoryWarningHelper
    {
        /// <summary>
        /// Invoked when the app's memory consumption has increased to a higher value.
        /// </summary>
        event EventHandler MemoryWarning;
    }
}
