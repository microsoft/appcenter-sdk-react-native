// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Crashes.Windows.Shared.Utils;
using System;
using Windows.System;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public class MemoryWarningHelper : IMemoryWarningHelper
    {
        private EventHandler _memoryWarning;
        public event EventHandler MemoryWarning
        {
            add
            {
                if (_memoryWarning == null)
                {
                    MemoryManager.AppMemoryUsageIncreased += OnAppMemoryUsageIncreased;
                }
                _memoryWarning += value;
            }
            remove
            {
                _memoryWarning -= value;
                if (_memoryWarning == null)
                {
                    MemoryManager.AppMemoryUsageIncreased -= OnAppMemoryUsageIncreased;
                }
            }
        }

        private void OnAppMemoryUsageIncreased(object sender, object e)
        {
            var level = MemoryManager.AppMemoryUsageLevel;
            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
            {
                _memoryWarning?.Invoke(sender, null);
            }
        }
    }
}
