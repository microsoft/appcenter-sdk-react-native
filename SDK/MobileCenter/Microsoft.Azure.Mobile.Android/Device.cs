using System;

namespace Microsoft.Azure.Mobile
{
    using AndroidDevice = Com.Microsoft.Azure.Mobile.Ingestion.Models.Device;

    public partial class Device
    {
        public Device(AndroidDevice device)
        {
            SDKName = device.SdkName;
            SDKVersion = device.SdkVersion;
            Model = device.Model;
            OEMName = device.OemName;
            OSName = device.OsName;
            OSVersion = device.OsVersion;
            OSBuild = device.OsBuild;
            OSApiLevel = device.OsApiLevel == null ? 0 : device.OsApiLevel.IntValue();
            Locale = device.Locale;
            TimeZoneOffset = device.TimeZoneOffset == null ? 0 : device.TimeZoneOffset.IntValue();
            ScreenSize = device.ScreenSize;
            AppVersion= device.AppVersion;
            CarrierName = device.CarrierName;
            CarrierCountry = device.CarrierCountry;
            AppBuild = device.AppBuild;
            AppNamespace = device.AppNamespace;
        }    
    }
}
