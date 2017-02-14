using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Reflection;

namespace Microsoft.Azure.Mobile.Utils
{
    public abstract class AbstractDeviceInformationHelper : IDeviceInformationHelper
    {
        public abstract event Action InformationInvalidated;

        public Ingestion.Models.Device GetDeviceInformation()
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
                LiveUpdatePackageHash = GetLiveUpdatePackageHash()
            };
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
            return this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
        }
        private string GetLocale()
        {
            return System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }
        private int GetTimeZoneOffset()
        {
            return (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes; //TODO unsure about this. base utc offset?
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
