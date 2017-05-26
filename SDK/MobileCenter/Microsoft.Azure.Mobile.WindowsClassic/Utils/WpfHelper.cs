using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    public static class WpfHelper
    {
        static WpfHelper()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                PresentationFramework =
                    assemblies.FirstOrDefault(assembly => assembly.GetName().Name == "PresentationFramework");
                IsRunningOnWpf = PresentationFramework != null;
            }
            catch (AppDomainUnloadedException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Unabled to determine whether this application is WPF or Windows Forms; proceeding as though it is Windows Forms.");
            }
        }

        public static bool IsRunningOnWpf { get; }
        public static Assembly PresentationFramework { get; }
    }
}
