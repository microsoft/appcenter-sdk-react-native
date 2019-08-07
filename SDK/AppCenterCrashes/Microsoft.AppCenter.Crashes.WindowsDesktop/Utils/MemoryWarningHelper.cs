using Microsoft.AppCenter.Crashes.Windows.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AppCenter.Crashes.Utils
{
    class MemoryWarningHelper : IMemoryWarningHelper
    {
        public event EventHandler MemoryWarning;
    }
}
