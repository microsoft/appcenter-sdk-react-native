using System;
using System.Collections.Generic;
using Android.App;
using Com.Microsoft.Azure.Mobile;
using Java.Lang;

namespace Microsoft.Azure.Mobile
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Com.Microsoft.Azure.Mobile.Utils.Async;
    using Java.Util;
    using AndroidWrapperSdk = Com.Microsoft.Azure.Mobile.Ingestion.Models.WrapperSdk;

    public partial class MobileCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "android";

        internal MobileCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var value = AndroidMobileCenter.LogLevel;
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
                    case Com.Microsoft.Azure.Mobile.Utils.MobileCenterLog.None:
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
                        androidValue = Com.Microsoft.Azure.Mobile.Utils.MobileCenterLog.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                AndroidMobileCenter.LogLevel = androidValue;
            }
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            AndroidMobileCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return AndroidMobileCenter.IsConfigured;
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            AndroidMobileCenter.Configure(SetWrapperSdkAndGetApplication(), appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            AndroidMobileCenter.Start(GetServices(services));
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            string parsedSecret;
            try
            {
                parsedSecret = GetSecretForPlatform(appSecret, PlatformIdentifier);
            }
            catch (ArgumentException ex)
            {
                MobileCenterLog.Assert(MobileCenterLog.LogTag, ex.Message);
                return;
            }
            AndroidMobileCenter.Start(SetWrapperSdkAndGetApplication(), parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.Run(() => (bool)AndroidMobileCenter.IsEnabled().Get());
        }

        static void PlatformSetEnabled(bool enabled)
        {
            AndroidMobileCenter.SetEnabled(enabled);
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            return Task.Run(() =>
            {
                var installId = AndroidMobileCenter.InstallId.Get() as Java.Util.UUID;
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
                xamarinAndroidVersion = xamarinAndroidVersion?.Split(';')[0];
            }
            var wrapperSdk = new AndroidWrapperSdk
            {
                WrapperSdkName = WrapperSdk.Name,
                WrapperSdkVersion = WrapperSdk.Version,
                WrapperRuntimeVersion = xamarinAndroidVersion
            };
            AndroidMobileCenter.SetWrapperSdk(wrapperSdk);
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
            AndroidMobileCenter.SetCustomProperties(customProperties.AndroidCustomProperties);
        }
    }
}