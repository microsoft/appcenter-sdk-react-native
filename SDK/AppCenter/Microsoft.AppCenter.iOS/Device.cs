using Microsoft.AppCenter.iOS.Bindings;

namespace Microsoft.AppCenter
{
    public partial class Device
    {
        public Device(MSDevice device)
        {
            SdkName = device.SdkName;
            SdkVersion = device.SdkVersion;
            Model = device.Model;
            OemName = device.OemName;
            OsName = device.OsName;
            OsVersion = device.OsVersion;
            OsBuild = device.OsBuild;
            OsApiLevel = device.OsApiLevel == null ? 0 : device.OsApiLevel.Int32Value;
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
