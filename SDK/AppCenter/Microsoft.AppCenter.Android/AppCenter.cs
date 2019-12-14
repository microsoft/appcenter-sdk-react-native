// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Android.App;
using Com.Microsoft.Appcenter;
using Java.Lang;

namespace Microsoft.AppCenter
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Com.Microsoft.Appcenter.Utils.Async;
    using Java.Util;
    using AndroidWrapperSdk = Com.Microsoft.Appcenter.Ingestion.Models.WrapperSdk;

    public partial class AppCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "android";

        internal AppCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var value = AndroidAppCenter.LogLevel;
                switch (value)
                {
                    case 2:
                        return LogLevel.Verbose;
                    case 3:
                        return LogLevel.Debug;
                    case 4:
                        return LogLevel.Info;
                    case 5:
                        return LogLevel.Warn;
                    case 6:
                        return LogLevel.Error;
                    case 7:
                        return LogLevel.Assert;
                    case Com.Microsoft.Appcenter.Utils.AppCenterLog.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }

            set
            {
                int androidValue;
                switch (value)
                {
                    case LogLevel.Verbose:
                        androidValue = 2;
                        break;
                    case LogLevel.Debug:
                        androidValue = 3;
                        break;
                    case LogLevel.Info:
                        androidValue = 4;
                        break;
                    case LogLevel.Warn:
                        androidValue = 5;
                        break;
                    case LogLevel.Error:
                        androidValue = 6;
                        break;
                    case LogLevel.Assert:
                        androidValue = 7;
                        break;
                    case LogLevel.None:
                        androidValue = Com.Microsoft.Appcenter.Utils.AppCenterLog.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                AndroidAppCenter.LogLevel = androidValue;
            }
        }

        static void PlatformSetUserId(string userId)
        {
            AndroidAppCenter.SetUserId(userId);
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            AndroidAppCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return AndroidAppCenter.IsConfigured;
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            AndroidAppCenter.Configure(SetWrapperSdkAndGetApplication(), appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            AndroidAppCenter.Start(GetServices(services));
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            string parsedSecret;
            try
            {
                parsedSecret = GetSecretAndTargetForPlatform(appSecret, PlatformIdentifier);
            }
            catch (AppCenterException ex)
            {
                AppCenterLog.Assert(AppCenterLog.LogTag, ex.Message);
                return;
            }
            AndroidAppCenter.Start(SetWrapperSdkAndGetApplication(), parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidAppCenter.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidAppCenter.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            var future = AndroidAppCenter.InstallId;
            return Task.Run(() =>
            {
                var installId = future.Get() as UUID;
                if (installId != null)
                {
                    return Guid.Parse(installId.ToString());
                }
                return (Guid?)null;
            });
        }

        static Application SetWrapperSdkAndGetApplication()
        {
            var monoAssembly = typeof(Java.Lang.Object).Assembly;
            var monoAssemblyAttibutes = monoAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), true);
            var monoAssemblyVersionAttibutes = monoAssemblyAttibutes as AssemblyInformationalVersionAttribute[];
            string xamarinAndroidVersion = null;
            if (monoAssemblyVersionAttibutes?.Length > 0)
            {
                xamarinAndroidVersion = monoAssemblyVersionAttibutes[0].InformationalVersion;
                var indexOfVersion = xamarinAndroidVersion?.IndexOf(';') ?? -1;
                if (indexOfVersion >= 0)
                {
                    xamarinAndroidVersion = xamarinAndroidVersion?.Substring(0, indexOfVersion + 1);
                }
            }
            var wrapperSdk = new AndroidWrapperSdk
            {
                WrapperSdkName = WrapperSdk.Name,
                WrapperSdkVersion = WrapperSdk.Version,
                WrapperRuntimeVersion = xamarinAndroidVersion
            };
            AndroidAppCenter.SetWrapperSdk(wrapperSdk);
            return (Application)Application.Context;
        }

        static Class[] GetServices(IEnumerable<Type> services)
        {
            var classes = new List<Class>();
            foreach (var t in services)
            {
                var propertyInfo = t.GetProperty("BindingType");
                if (propertyInfo != null)
                {
                    var value = (Type)propertyInfo.GetValue(null, null);
                    if (value != null)
                    {
                        var aClass = Class.FromType((Type)propertyInfo.GetValue(null, null));
                        if (aClass != null)
                        {
                            classes.Add(aClass);
                        }
                    }
                }
            }
            return classes.ToArray();
        }

        static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            AndroidAppCenter.SetCustomProperties(customProperties.AndroidCustomProperties);
        }

        internal static void PlatformUnsetInstance()
        {
            AndroidAppCenter.UnsetInstance();
        }
    }
}
