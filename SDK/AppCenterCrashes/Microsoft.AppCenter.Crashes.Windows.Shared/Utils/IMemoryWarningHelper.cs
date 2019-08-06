using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AppCenter.Crashes.Windows.Shared.Utils
{
    interface IMemoryWarningHelper
    {
        event EventHandler  MemoryWarning;

        bool GetHasReceiveMemoryWarning { get; }

        void PrepareMemoryWarning();
    }
}
