using System;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using System.Reflection;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        private bool _leftBackground;
        private string _cachedScreenSize;
        public override event EventHandler InformationInvalidated;

        public DeviceInformationHelper()
        {
            CoreApplication.LeavingBackground += (o, e) => {
                if (_leftBackground) return;
                _leftBackground = true;
                CacheScreenSize();
                DisplayInformation.DisplayContentsInvalidated += (displayInfo, obj) =>
                {
                    _cachedScreenSize = ScreenSizeFromDisplayInfo(displayInfo);
                    InformationInvalidated?.Invoke(this, EventArgs.Empty);
                };
                InformationInvalidated?.Invoke(this, EventArgs.Empty);
            };
        }

        private void CacheScreenSize()
        {
            try
            {
                var displayInfo = DisplayInformation.GetForCurrentView();
                _cachedScreenSize = ScreenSizeFromDisplayInfo(displayInfo);
            }
            catch (Exception e)
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Failed to retrieve screen size", e);
            }
        }

        private string ScreenSizeFromDisplayInfo(DisplayInformation displayInfo)
        {
            return $"{displayInfo.ScreenWidthInRawPixels}x{displayInfo.ScreenHeightInRawPixels}";
        }

        protected override string GetSdkName()
        {
            return "mobilecenter.uwp";
        }

        protected override string GetDeviceModel()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            return deviceInfo.SystemProductName;
        }

        protected override string GetAppNamespace()
        {
            return Application.Current.GetType().Namespace;
        }

        protected override string GetDeviceOemName()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            return deviceInfo.SystemManufacturer;
        }

        protected override string GetOsName()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            return deviceInfo.OperatingSystem;
        }

        protected override string GetOsBuild()
        {
            /* Adapted from https://social.msdn.microsoft.com/Forums/en-US/2d8a7dab-1bad-4405-b70d-768e4cb2af96/uwp-get-os-version-in-an-uwp-app?forum=wpdevelop */
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            return build.ToString();
        }

        protected override string GetOsVersion()
        {
            /* Adapted from https://social.msdn.microsoft.com/Forums/en-US/2d8a7dab-1bad-4405-b70d-768e4cb2af96/uwp-get-os-version-in-an-uwp-app?forum=wpdevelop */
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong major = (version & 0xFFFF000000000000L) >> 48;
            ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            return $"{major}.{minor}";
        }

        protected override string GetAppVersion()
        {
            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            return $"{packageVersion.Major}.{packageVersion.Minor}";
        }

        protected override string GetAppBuild()
        {
            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            return packageVersion.Build.ToString();
        }

        protected override string GetScreenSize()
        {
            return _cachedScreenSize;
        }
    }
}
