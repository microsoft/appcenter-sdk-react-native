// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    using AndroidDevice = Com.Microsoft.Appcenter.Ingestion.Models.Device;

    public partial class Device
    {
        public Device(AndroidDevice device)
        {
            SdkName = device.SdkName;
            SdkVersion = device.SdkVersion;
            Model = device.Model;
            OemName = device.OemName;
            OsName = device.OsName;
            OsVersion = device.OsVersion;
            OsBuild = device.OsBuild;
            OsApiLevel = device.OsApiLevel == null ? 0 : device.OsApiLevel.IntValue();
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
