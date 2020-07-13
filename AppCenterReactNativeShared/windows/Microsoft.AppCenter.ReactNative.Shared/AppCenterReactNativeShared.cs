using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace Microsoft.AppCenter.ReactNative.Shared
{
    public class AppCenterReactNativeShared
    {
        private const string AppCenterConfigAsset = "appcenter-config.json";

        private const string AppSecretKey = "app_secret";

        private const string StartAutomaticallyKey = "start_automatically";

        private static bool _configured = false;

        private static JsonObject _configuration = null;

        private static string _appSecret = null;

        private static bool _startAutomatically;

        public async static void ConfigureAppCenter()
        {
            if (_configured)
            {
                return;
            }
            _configured = true;
            var wrapperSdk = new Microsoft.AppCenter.WrapperSdk(
                "appcenter.react-native",
                "3.0.3");
            Microsoft.AppCenter.AppCenter.SetWrapperSdk(wrapperSdk);
            await ReadConfigurationFile();
            if (!_startAutomatically)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Configure not to start automatically.");
                return;
            }

            if (_appSecret != null && _appSecret.Length != 0)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Configure without secret.");
            }
            else
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Configure with secret.");
                AppCenter.Configure(_appSecret);
            }
        }

        private async static Task ReadConfigurationFile()
        {
            try
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Reading " + AppCenterConfigAsset);
                var file = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri("ms-appx:///Assets/" + AppCenterConfigAsset));
                var content = await FileIO.ReadTextAsync(file);
                _configuration = JsonObject.Parse(content);
                if (_appSecret == null)
                {
                    _appSecret = _configuration.GetNamedString(AppSecretKey);
                    _startAutomatically = _configuration.GetNamedBoolean(StartAutomaticallyKey, true);
                }
            }
            catch (Exception e)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Failed to parse appcenter-config.json", e);
                _configuration = new JsonObject();
            }
        }

        public static void SetAppSecret(String secret)
        {
            _appSecret = secret;
        }

        public static void SetStartAutomatically(bool startAutomatically)
        {
            _startAutomatically = startAutomatically;
        }

        public static JsonObject getConfiguration()
        {
            return _configuration;
        }
    }
}