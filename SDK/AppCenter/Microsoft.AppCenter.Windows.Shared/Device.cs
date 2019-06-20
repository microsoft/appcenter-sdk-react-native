// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class Device
    {
        /// <summary>
        /// Creates a public device model from an ingestion device model.
        /// </summary>
        /// <param name="device">The ingestion device model.</param>
        public Device(Ingestion.Models.Device device)
        {
            SdkName = device.SdkName;
            SdkVersion = device.SdkVersion;
            Model = device.Model;
            OemName = device.OemName;
            OsName = device.OsName;
            OsVersion = device.OsVersion;
            OsBuild = device.OsBuild;
            OsApiLevel = device.OsApiLevel;
            Locale = device.Locale;
            TimeZoneOffset = device.TimeZoneOffset;
            ScreenSize = device.ScreenSize;
            AppVersion = device.AppVersion;
            CarrierName = device.CarrierName;
            CarrierCountry = device.CarrierCountry;
            AppBuild = device.AppBuild;
            AppNamespace = device.AppNamespace;
        }
    }
}