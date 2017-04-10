using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Distribute
{
    using iOSDistribute = iOS.Bindings.MSDistribute;

    public static partial class Distribute
    {
        static Type _internalBindingType = typeof(iOSDistribute);

        [Preserve]
        public static Type BindingType
        {
            get
            {
                return _internalBindingType;
            }
        }

        static bool PlatformEnabled
        {
            get
            {
                return iOSDistribute.IsEnabled();
            }

            set
            {
                iOSDistribute.SetEnabled(value);
            }
        }
        static void PlatformSetInstallUrl(string installUrl)
        {
            iOSDistribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            iOSDistribute.SetApiUrl(apiUrl);
        }

        /// <summary>
        /// Process URL request for the Distribute service.
        /// Place this method call into app delegate openUrl method.
        /// </summary>
        /// <param name="url">The url with parameters.</param>
        public static void OpenUrl(NSUrl url)
        {
            iOSDistribute.OpenUrl(url);
        }

        /// <summary>
        /// Do not check for updates in case the app is launched with a debug configuration.
        /// Place this method call into you app delegate's didFinishLaunching:withOptions: method BEFORE 
        /// you call MobileCenter.start(...) if you are using in-app updates.
        /// </summary>
        /// <remarks>
        /// This method is required because the SDK cannot detect an attached debugger, nor can it detect
        /// a release configuration. If this method is not called, the browser will appear and try to
        /// setup in-app updates which we do not want.
        /// </remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DontCheckForUpdatesInDebug()
        {
            _internalBindingType = null;
        }
    }
}
