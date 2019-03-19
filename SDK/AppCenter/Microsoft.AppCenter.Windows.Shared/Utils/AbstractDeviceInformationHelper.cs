// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter.Utils
{
    public abstract class AbstractDeviceInformationHelper : IDeviceInformationHelper
    {
        public virtual async Task<Ingestion.Models.Device> GetDeviceInformationAsync()
        {
            var device = new Ingestion.Models.Device
            {
                SdkName = GetSdkName(),
                SdkVersion = GetSdkVersion(),
                Model = GetDeviceModel(),
                OemName = GetDeviceOemName(),
                OsName = GetOsName(),
                OsVersion = GetOsVersion(),
                Locale = GetLocale(),
                TimeZoneOffset = GetTimeZoneOffset(),
                ScreenSize = GetScreenSize(),
                AppVersion = GetAppVersion(),
                AppBuild = GetAppBuild(),
                WrapperSdkVersion = GetWrapperSdkVersion(),
                WrapperSdkName = GetWrapperSdkName(),
                OsBuild = GetOsBuild(),
                OsApiLevel = GetOsApiLevel(),
                CarrierName = GetCarrierName(),
                CarrierCountry = GetCarrierCountry(),
                AppNamespace = GetAppNamespace(),
                LiveUpdateReleaseLabel = GetLiveUpdateReleaseLabel(),
                LiveUpdateDeploymentKey = GetLiveUpdateDevelopmentKey(),
                LiveUpdatePackageHash = GetLiveUpdatePackageHash()
            };

            return await Task<Ingestion.Models.Device>.Factory.StartNew(() => device).ConfigureAwait(false);
        }

        protected abstract string GetSdkName();
        protected abstract string GetDeviceModel();
        protected abstract string GetDeviceOemName();
        protected abstract string GetOsName();
        protected abstract string GetOsBuild();
        protected abstract string GetOsVersion();
        protected abstract string GetAppVersion();
        protected abstract string GetAppBuild();
        protected abstract string GetScreenSize();

        protected virtual string GetCarrierName()
        {
            return null;
        }
        protected virtual string GetCarrierCountry()
        {
            return null;
        }
        protected virtual string GetAppNamespace()
        {
            return null;
        }
        protected virtual string GetLiveUpdateReleaseLabel()
        {
            return null;
        }
        protected virtual string GetLiveUpdateDevelopmentKey()
        {
            return null;
        }

        protected virtual string GetLiveUpdatePackageHash()
        {
            return null;
        }

        private string GetSdkVersion()
        {
           return GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        private string GetLocale()
        {
            return CultureInfoHelper.GetCurrentCulture().Name;
        }

        private int GetTimeZoneOffset()
        {
            return (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
        }

        private string GetWrapperSdkVersion()
        {
            return null;
        }
        private string GetWrapperSdkName()
        {
            return null;
        }
        private int? GetOsApiLevel()
        {
            return null;
        }
    }
}
