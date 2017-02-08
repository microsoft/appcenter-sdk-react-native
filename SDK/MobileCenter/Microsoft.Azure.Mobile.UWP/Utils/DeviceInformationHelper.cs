using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Microsoft.Azure.Mobile.UWP.Utils
{
    //public Device(string sdkVersion, string wrapperSdkVersion = default(string), string wrapperSdkName = default(string), string osBuild = default(string), int? osApiLevel = default(int?), string carrierName = default(string), string carrierCountry = default(string), string appNamespace = default(string), string liveUpdateReleaseLabel = default(string), string liveUpdateDeploymentKey = default(string), string liveUpdatePackageHash = default(string))

    public static class DeviceInformationHelper
    {
        private const string SdkName = "mobilecenter.uwp";
        //TODO thread safety?
        public static Ingestion.Models.Device GetDeviceInformation()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            string model = deviceInfo.FriendlyName;
            string oemName = deviceInfo.SystemManufacturer;
            string osName = deviceInfo.OperatingSystem;
            string osVersion = deviceInfo.SystemFirmwareVersion; //TODO unsure about this one
            string locale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName; //TODO unsure about this one
            int timeZoneOffset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes; //TODO unsure about this one

            var displayInfo = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            string screenSize = displayInfo.ScreenWidthInRawPixels.ToString() + "x" + displayInfo.ScreenHeightInRawPixels.ToString();  //{width}x{height}
            string appVersion = Windows.ApplicationModel.Package.Current.Id.Version.ToString(); //TODO unsure about this one
            string appBuild = Windows.ApplicationModel.Package.Current.Id.Version.Build.ToString();
        }
    }
}
