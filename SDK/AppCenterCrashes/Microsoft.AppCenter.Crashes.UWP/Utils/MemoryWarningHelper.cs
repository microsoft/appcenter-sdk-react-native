using Microsoft.AppCenter.Crashes.Windows.Shared.Utils;
using Microsoft.AppCenter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace Microsoft.AppCenter.Crashes.Utils
{
    class MemoryWarningHelper : IMemoryWarningHelper
    {
        public const string LogTag = "AppCenterCrashes";

        private static bool HasReceivedMemoryWarning;

        internal const string PrefKeyMemoryWarning = Constants.KeyPrefix + "MemoryWarning";

        bool IMemoryWarningHelper.GetHasReceiveMemoryWarning => HasReceivedMemoryWarning;

        public event EventHandler MemoryWarning
        {
            add
            {
                MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;
            }
            remove
            {
                MemoryManager.AppMemoryUsageIncreased -= MemoryManager_AppMemoryUsageIncreased;
                ApplicationData.Current.LocalSettings.Values[PrefKeyMemoryWarning] = false;
            }
        }

        public void PrepareMemoryWarning()
        {
            var memoryWarning = ApplicationData.Current.LocalSettings.Values[PrefKeyMemoryWarning];
            if ((memoryWarning as bool?) == true)
            {
                HasReceivedMemoryWarning = true;
                AppCenterLog.Debug(LogTag, "The application received a low memory warning in the last session.");
            }
            ApplicationData.Current.LocalSettings.Values[PrefKeyMemoryWarning] = false;
        }

        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            var level = MemoryManager.AppMemoryUsageLevel;
            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
            {
                ApplicationData.Current.LocalSettings.Values[PrefKeyMemoryWarning] = true;
                AppCenterLog.Debug(LogTag, "The application received a low memory warning.");
            }
        }
    }
}
