using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;
using System.Reflection;
using Windows.ApplicationModel.Core;
using System.Threading;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DeviceInformationHelper : IDeviceInformationHelper//TODO finish this
    {
        private const string SdkName = "mobilecenter.uwp";
        private string _screenSize = "";
        private SemaphoreSlim _mutex = new SemaphoreSlim(0, 1);
        private bool _hasDisplayInformation = false;
        //TODO thread safety?

        public DeviceInformationHelper()
        {
            CoreApplication.LeavingBackground += (o, e) => GetDisplayInformation();
        }

        public async Task<Ingestion.Models.Device> GetDeviceInformationAsync()
        {
            var device = GetDeviceInformation();
            if (!_hasDisplayInformation)
            {
                await _mutex.WaitAsync();
            }
            device.ScreenSize = _screenSize;
            return device;
        }

        private void GetDisplayInformation()
        {
            var displayInfo = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            _screenSize = displayInfo.ScreenWidthInRawPixels.ToString() + "x" + displayInfo.ScreenHeightInRawPixels.ToString();
            _hasDisplayInformation = true;
            _mutex.Release();
        }

        public Ingestion.Models.Device GetDeviceInformation()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            string model = deviceInfo.SystemProductName;
            string oemName = deviceInfo.SystemManufacturer;
            string osName = deviceInfo.OperatingSystem;

            /* From https://social.msdn.microsoft.com/Forums/en-US/2d8a7dab-1bad-4405-b70d-768e4cb2af96/uwp-get-os-version-in-an-uwp-app?forum=wpdevelop */
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong major = (version & 0xFFFF000000000000L) >> 48;
            ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            //ulong revision = (version & 0x000000000000FFFFL);
            string osVersion = $"{major}.{minor}";
            string osBuild = build.ToString();
            
            string sdkVersion = this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
            string locale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            int timeZoneOffset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes; //TODO unsure about this one
            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            string appVersion = $"{packageVersion.Major}.{packageVersion.Minor}"; //TODO unsure about this one
            string appBuild = packageVersion.Build.ToString();
            return new Ingestion.Models.Device(SdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, _screenSize, appVersion, appBuild, null, null, osBuild);
        }

    }
}
