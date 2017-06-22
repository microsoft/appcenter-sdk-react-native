using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;

namespace Microsoft.Azure.Mobile
{
    using iOSLogLevel = Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel;
    using iOSMobileCenter = Microsoft.Azure.Mobile.iOS.Bindings.MSMobileCenter;
    using iOSWrapperSdk = Microsoft.Azure.Mobile.iOS.Bindings.MSWrapperSdk;

    public partial class MobileCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "ios";

        internal MobileCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var val = iOSMobileCenter.LogLevel();
                switch (val)
                {
                    case iOSLogLevel.Verbose:
                        return LogLevel.Verbose;
                    case iOSLogLevel.Debug:
                        return LogLevel.Debug;
                    case iOSLogLevel.Info:
                        return LogLevel.Info;
                    case iOSLogLevel.Warning:
                        return LogLevel.Warn;
                    case iOSLogLevel.Error:
                        return LogLevel.Error;
                    case iOSLogLevel.Assert:
                        return LogLevel.Assert;
                    case iOSLogLevel.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
            }
            set
            {
                iOSLogLevel loglevel;
                switch (value)
                {
                    case LogLevel.Verbose:
                        loglevel = iOSLogLevel.Verbose;
                        break;
                    case LogLevel.Debug:
                        loglevel = iOSLogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        loglevel = iOSLogLevel.Info;
                        break;
                    case LogLevel.Warn:
                        loglevel = iOSLogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        loglevel = iOSLogLevel.Error;
                        break;
                    case LogLevel.Assert:
                        loglevel = iOSLogLevel.Assert;
                        break;
                    case LogLevel.None:
                        loglevel = iOSLogLevel.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                iOSMobileCenter.SetLogLevel(loglevel);
            }
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            iOSMobileCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return iOSMobileCenter.IsConfigured();
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            SetWrapperSdk();
            iOSMobileCenter.ConfigureWithAppSecret(appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            SetWrapperSdk();
            foreach (var service in GetServices(services))
            {
                iOSMobileCenter.StartService(service);
            }
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            SetWrapperSdk();
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
            iOSMobileCenter.Start(parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(iOSMobileCenter.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            iOSMobileCenter.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            Guid? installId = Guid.Parse(iOSMobileCenter.InstallId().AsString());
            return Task.FromResult(installId);
        }

        static Class[] GetServices(IEnumerable<Type> services)
        {
            var classes = new List<Class>();
            foreach (var t in services)
            {
                var bindingType = GetBindingType(t);
                if (bindingType != null)
                {
                    var aClass = GetClassForType(bindingType);
                    if (aClass != null)
                    {
                        classes.Add(aClass);
                    }
                }
            }
            return classes.ToArray();
        }

        static Class GetClassForType(Type type)
        {
            IntPtr classHandle = Class.GetHandle(type);
            if (classHandle != IntPtr.Zero)
            {
                return new Class(classHandle);
            }
            return null;
        }

        static Type GetBindingType(Type type)
        {
            return (Type)type.GetProperty("BindingType").GetValue(null, null);
        }

        static void SetWrapperSdk()
        {
            iOSWrapperSdk wrapperSdk = new iOSWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, Constants.Version, null, null, null);
            iOSMobileCenter.SetWrapperSdk(wrapperSdk);
        }

        static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            iOSMobileCenter.SetCustomProperties(customProperties.IOSCustomProperties);
        }
    }
}
