using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Java.Lang;

namespace Microsoft.Sonoma.Xamarin.Core
{
    using AndroidSonoma = Com.Microsoft.Sonoma.Core.Sonoma;
    using AndroidWrapperSdk = Com.Microsoft.Sonoma.Core.Ingestion.Models.WrapperSdk;

    public static class Sonoma
    {

        public static LogLevel LogLevel
        {
            get
            {
                var value = AndroidSonoma.LogLevel;
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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                AndroidSonoma.LogLevel = androidValue;
            }
        }

        public static void SetServerUrl(string serverUrl)
        {
            AndroidSonoma.SetServerUrl(serverUrl);
        }

        public static void Initialize(string appSecret)
        {
            AndroidSonoma.Initialize(SetWrapperSdkAndGetApplication(), appSecret);
        }

        public static void Start(params Type[] features)
        {
            AndroidSonoma.Start(GetFeatures(features));
        }

        public static void Start(string appSecret, params Type[] features)
        {
            AndroidSonoma.Start(SetWrapperSdkAndGetApplication(), appSecret, GetFeatures(features));
        }
        
        private static Application SetWrapperSdkAndGetApplication()
        {
            AndroidSonoma.SetWrapperSdk(new AndroidWrapperSdk { WrapperSdkName = WrapperSdk.Name, WrapperSdkVersion = WrapperSdk.Version });
            return (Application)Application.Context;
        }

        private static Class[] GetFeatures(IEnumerable<Type> features)
        {
            return features.Select(feature => Class.FromType((Type)feature.GetProperty("BindingType").GetValue(null, null))).ToArray();
        }

        public static bool Enabled
        {
            get { return AndroidSonoma.Enabled; }
            set { AndroidSonoma.Enabled = value; }
        }

        public static Guid InstallId => Guid.Parse(AndroidSonoma.InstallId.ToString());
    }
}