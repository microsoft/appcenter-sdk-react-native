// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Crashes.Windows.Shared.Utils
{
    /// <summary>
    /// The interface for platform specific memory warning event.
    /// </summary>
    public interface IMemoryWarningHelper
    {
        /// <summary>
        /// Invoked when the app receives a memory warning.
        /// </summary>
        event EventHandler MemoryWarning;
    }
}
