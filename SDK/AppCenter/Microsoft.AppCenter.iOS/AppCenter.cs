// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;

namespace Microsoft.AppCenter
{
    using iOSLogLevel = Microsoft.AppCenter.iOS.Bindings.MSLogLevel;
    using iOSAppCenter = Microsoft.AppCenter.iOS.Bindings.MSAppCenter;
    using iOSWrapperSdk = Microsoft.AppCenter.iOS.Bindings.MSWrapperSdk;

    public partial class AppCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "ios";

        internal AppCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var val = iOSAppCenter.LogLevel();
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
                iOSAppCenter.SetLogLevel(loglevel);
            }
        }

        static void PlatformSetUserId(string userId)
        {
            iOSAppCenter.SetUserId(userId);
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            iOSAppCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return iOSAppCenter.IsConfigured();
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            SetWrapperSdk();
            iOSAppCenter.ConfigureWithAppSecret(appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            SetWrapperSdk();
            foreach (var service in GetServices(services))
            {
                iOSAppCenter.StartService(service);
            }
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            SetWrapperSdk();
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
            iOSAppCenter.Start(parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(iOSAppCenter.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            iOSAppCenter.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            Guid? installId = Guid.Parse(iOSAppCenter.InstallId().AsString());
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
            return (Type)type.GetProperty("BindingType")?.GetValue(null, null);
        }

        static void SetWrapperSdk()
        {
            iOSWrapperSdk wrapperSdk = new iOSWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, Constants.Version, null, null, null);
            iOSAppCenter.SetWrapperSdk(wrapperSdk);
        }

        static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            iOSAppCenter.SetCustomProperties(customProperties?.IOSCustomProperties);
        }

        internal static void PlatformUnsetInstance()
        {
            iOSAppCenter.ResetSharedInstance();
        }
    }
}
