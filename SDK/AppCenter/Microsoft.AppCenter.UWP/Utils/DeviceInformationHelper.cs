// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        private static IScreenSizeProvider ScreenSizeProvider;
        private static IScreenSizeProviderFactory ScreenSizeProviderFactory =
            new DefaultScreenSizeProviderFactory();

        // This method must be called before instance of DeviceInformationHelper has been created
        // for a custom screen size provider to be used.
        public static void SetScreenSizeProviderFactory(IScreenSizeProviderFactory factory)
        {
            ScreenSizeProviderFactory = factory;
            ScreenSizeProvider = null;
        }

        public DeviceInformationHelper()
        {
            lock (ScreenSizeProviderFactory)
            {
                if (ScreenSizeProvider == null)
                {
                    ScreenSizeProvider = ScreenSizeProviderFactory.CreateScreenSizeProvider();
                    ScreenSizeProvider.ScreenSizeChanged += (sender, e) =>
                    {
                        InvalidateInformation(sender, e);
                    };
                }
            }
        }

        public override async Task<Ingestion.Models.Device> GetDeviceInformationAsync()
        {
            await ScreenSizeProvider.WaitUntilReadyAsync().ConfigureAwait(false);
            return await base.GetDeviceInformationAsync().ConfigureAwait(false);
        }

        protected override string GetSdkName()
        {
            return "appcenter.uwp";
        }

        protected override string GetDeviceModel()
        {
            var deviceInfo = new EasClientDeviceInformation();
            return string.IsNullOrEmpty(deviceInfo.SystemProductName) ? deviceInfo.SystemSku : deviceInfo.SystemProductName;
        }

        protected override string GetAppNamespace()
        {
            return Package.Current.Id.Name;
        }

        protected override string GetDeviceOemName()
        {
            var deviceInfo = new EasClientDeviceInformation();
            return deviceInfo.SystemManufacturer;
        }

        protected override string GetOsName()
        {
            var deviceInfo = new EasClientDeviceInformation();
            return deviceInfo.OperatingSystem;
        }

        protected override string GetOsBuild()
        {
            // Adapted from https://social.msdn.microsoft.com/Forums/en-US/2d8a7dab-1bad-4405-b70d-768e4cb2af96/uwp-get-os-version-in-an-uwp-app?forum=wpdevelop
            var deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            var version = ulong.Parse(deviceFamilyVersion);
            var major = (version & 0xFFFF000000000000L) >> 48;
            var minor = (version & 0x0000FFFF00000000L) >> 32;
            var build = (version & 0x00000000FFFF0000L) >> 16;
            var revision = version & 0x000000000000FFFFL;
            return $"{major}.{minor}.{build}.{revision}";
        }

        protected override string GetOsVersion()
        {
            // Adapted from https://social.msdn.microsoft.com/Forums/en-US/2d8a7dab-1bad-4405-b70d-768e4cb2af96/uwp-get-os-version-in-an-uwp-app?forum=wpdevelop
            var deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            var version = ulong.Parse(deviceFamilyVersion);
            var major = (version & 0xFFFF000000000000L) >> 48;
            var minor = (version & 0x0000FFFF00000000L) >> 32;
            var build = (version & 0x00000000FFFF0000L) >> 16;
            return $"{major}.{minor}.{build}";
        }

        protected override string GetAppVersion()
        {
            var packageVersion = Package.Current.Id.Version;
            return $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";
        }

        protected override string GetAppBuild()
        {
            var packageVersion = Package.Current.Id.Version;
            return $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";
        }

        protected override string GetScreenSize()
        {
            return ScreenSizeProvider.ScreenSize;
        }
    }
}
