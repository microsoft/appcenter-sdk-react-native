using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AppCenter.Crashes.Windows.Shared.Utils
{
    public interface IMemoryWarningHelper
    {
        event EventHandler MemoryWarning;
    }
}
