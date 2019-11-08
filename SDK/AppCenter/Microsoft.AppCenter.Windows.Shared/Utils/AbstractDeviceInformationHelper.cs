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
        public static string DefaultSystemManufacturer = "System manufacturer";
        public static string DefaultSystemProductName = "System Product Name";
        public static string DefaultSystemSku = "SKU";
        public static event EventHandler InformationInvalidated;
        private static string _country;
        private static WrapperSdk _wrapperSdk;

        internal static void SetCountryCode(string country)
        {
            _country = country;
            InvalidateInformation(null, EventArgs.Empty);
        }

        public virtual Task<Ingestion.Models.Device> GetDeviceInformationAsync()
        {
            return Task.FromResult(GetDeviceInformation());
        }

        public virtual Ingestion.Models.Device GetDeviceInformation()
        {
            return new Ingestion.Models.Device
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
                LiveUpdatePackageHash = GetLiveUpdatePackageHash(),
                WrapperRuntimeVersion = GetWrapperRuntimeVersion()
            };
        }

        internal static void SetWrapperSdk(WrapperSdk wrapperSdk)
        {
            _wrapperSdk = wrapperSdk;
        }

        protected static void InvalidateInformation(object sender, EventArgs e)
        {
            InformationInvalidated?.Invoke(sender, e);
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
            return _country;
        }

        protected virtual string GetAppNamespace()
        {
            return null;
        }

        protected virtual string GetLiveUpdateReleaseLabel()
        {
            if (_wrapperSdk != null)
            {
                return _wrapperSdk.LiveUpdateReleaseLabel;
            }
            return null;
        }

        protected virtual string GetLiveUpdateDevelopmentKey()
        {
            if (_wrapperSdk != null)
            {
                return _wrapperSdk.LiveUpdateDeploymentKey;
            }
            return null;
        }

        protected virtual string GetLiveUpdatePackageHash()
        {
            if (_wrapperSdk != null)
            {
                return _wrapperSdk.LiveUpdatePackageHash;
            }
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
            if (_wrapperSdk != null)
            {
                return _wrapperSdk.WrapperSdkVersion;
            }
            return null;
        }
        private string GetWrapperSdkName()
        {
            if(_wrapperSdk != null)
            {
                return _wrapperSdk.WrapperSdkName;
            }
            return null;
        }

        private string GetWrapperRuntimeVersion()
        {
            if (_wrapperSdk != null)
            {
                return _wrapperSdk.WrapperRuntimeVersion;
            }
            return null;
        }

        private int? GetOsApiLevel()
        {
            return null;
        }
    }
}
