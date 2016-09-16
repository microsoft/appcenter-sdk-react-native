using System;
using System.Linq;
using System.Reflection;
using Android.App;
using Com.Microsoft.Sonoma.Core.Ingestion.Models;
using Java.Lang;

namespace Microsoft.Sonoma.Xamarin.Core
{
    using AndroidSonoma = Com.Microsoft.Sonoma.Core.Sonoma;

    public static class Sonoma
    {
        public static bool Enabled
        {
            get { return AndroidSonoma.Enabled; }
            set { AndroidSonoma.Enabled = value; }
        }

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

        public static Guid InstallId => Guid.Parse(AndroidSonoma.InstallId.ToString());

        public static void Start(string appSecret, params Type[] features)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AndroidSonoma.SetWrapperSdk(new WrapperSdk { WrapperSdkName = "sonoma.xamarin", WrapperSdkVersion = version });
            var bindingFeatures = features.Select(feature => Class.FromType((Type)feature.GetMethod("GetBindingType").Invoke(null, null))).ToArray();
            var application = (Application)Application.Context;
            AndroidSonoma.Start(application, appSecret, bindingFeatures);
        }
    }
}