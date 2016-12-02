using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{   
    public static partial class Crashes
    {
        /// <summary>
        /// This method is for internal SDK use only. Do not call.
        /// </summary>
        public static void ApplyDelegate()
        {
            /* Peform custom setup around the native SDK's for setting signal handlers */
            MSWrapperExceptionManager.SetDelegate(new CrashesInitializationDelegate());
        }
    }
}
