using System;

namespace Microsoft.Azure.Mobile
{
    using iOS.Bindings;

    public partial class Device
    {
        public Device(MSDevice device)
        {
            SDKName = device.SdkName;
            SDKVersion = device.SdkVersion;
            Model = device.Model;
            OEMName = device.OemName;
            OSName = device.OsName;
            OSVersion = device.OsVersion;
            OSBuild = device.OsBuild;
            OSApiLevel = device.OsApiLevel == null ? 0 : device.OsApiLevel.Int32Value;
            Locale = device.Locale;
            TimeZoneOffset = device.TimeZoneOffset == null ? 0 : device.TimeZoneOffset.Int32Value;
            ScreenSize = device.ScreenSize;
            AppVersion= device.AppVersion;
            CarrierName = device.CarrierName;
            CarrierCountry = device.CarrierCountry;
            AppBuild = device.AppBuild;
            AppNamespace = device.AppNamespace;
        }
    }
}
