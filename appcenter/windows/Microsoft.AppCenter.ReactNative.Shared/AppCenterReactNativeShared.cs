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

        private static bool _configuring = false;

        private static JsonObject _configuration = null;

        private static string _appSecret = null;

        private static bool _startAutomatically;

        private static object _lockObject = new object();

        private static TaskCompletionSource<bool> _taskCompetionSource;

        public async static Task<Task> ConfigureAppCenter()
        {
            lock(_lockObject)
            {
                if (_configuring)
                {
                    return _taskCompetionSource.Task;
                }
                _taskCompetionSource = new TaskCompletionSource<bool>();
                _configuring = true;
            }
            
            try
            {
                var wrapperSdk = new Microsoft.AppCenter.WrapperSdk(
                    "appcenter.react-native",
                    "3.0.3");
                Microsoft.AppCenter.AppCenter.SetWrapperSdk(wrapperSdk);
                await ReadConfigurationFile();
                if (!_startAutomatically)
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag, "Configure not to start automatically.");
                }
                else
                {
                    if (_appSecret == null || _appSecret.Length == 0)
                    {
                        // .NET SDK does not allow the user to configure AppCenter without an app secret.
                        AppCenterLog.Debug(AppCenterLog.LogTag, "Configure without secret.");
                    }
                    else
                    {
                        AppCenterLog.Debug(AppCenterLog.LogTag, "Configure with secret.");
                        AppCenter.Configure(_appSecret);
                    }
                }
                _taskCompetionSource.SetResult(true);
            }
            catch(Exception e)
            {
                _taskCompetionSource.SetException(e);
            }
            return _taskCompetionSource.Task;
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
                throw;
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