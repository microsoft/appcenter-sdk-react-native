using System;
using System.Linq;
using Android.App;
using Java.Lang;

// ReSharper disable once CheckNamespace
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
            var bindingFeatures = features.Select(feature => Class.FromType((Type) feature.GetMethod("GetBindingType").Invoke(null, null))).ToArray();
            var application = (Application) Application.Context;
            AndroidSonoma.Start(application, appSecret, bindingFeatures);
        }
    }
}