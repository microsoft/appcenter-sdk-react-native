// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Crashes.Windows.Shared.Utils;
using System;

namespace Microsoft.AppCenter.Crashes.Utils
{
    class MemoryWarningHelper : IMemoryWarningHelper
    {
        public event EventHandler MemoryWarning;
    }
}
