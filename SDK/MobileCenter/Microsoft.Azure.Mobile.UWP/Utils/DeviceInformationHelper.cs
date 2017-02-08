using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Microsoft.Azure.Mobile.Utils
{
    public static class DeviceInformationHelper //TODO finish this
    {
        private const string SdkName = "mobilecenter.uwp";
        //TODO thread safety?
        public static Ingestion.Models.Device GetDeviceInformation()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            string model = deviceInfo.FriendlyName;
            string oemName = deviceInfo.SystemManufacturer;
            string osName = deviceInfo.OperatingSystem;

            /* From https://social.msdn.microsoft.com/Forums/en-US/2d8a7dab-1bad-4405-b70d-768e4cb2af96/uwp-get-os-version-in-an-uwp-app?forum=wpdevelop */
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong major = (version & 0xFFFF000000000000L) >> 48;
            ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            ulong revision = (version & 0x000000000000FFFFL);
            string osVersion = $"{major}.{minor}.{build}.{revision}"; //TODO very unsure about this one


            string locale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName; //TODO unsure about this one
            int timeZoneOffset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes; //TODO unsure about this one

            var displayInfo = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            string screenSize = displayInfo.ScreenWidthInRawPixels.ToString() + "x" + displayInfo.ScreenHeightInRawPixels.ToString();  //{width}x{height}
            string appVersion = Windows.ApplicationModel.Package.Current.Id.Version.ToString(); //TODO unsure about this one
            string appBuild = Windows.ApplicationModel.Package.Current.Id.Version.Build.ToString();
            return new Ingestion.Models.Device(SdkName, "sdkVersion", model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
        }
    }
}
