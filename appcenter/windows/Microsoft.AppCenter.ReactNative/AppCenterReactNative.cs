using Microsoft.AppCenter;
using Microsoft.AppCenter.ReactNative.Shared;
using Microsoft.ReactNative.Managed;
using System;

namespace Microsoft.AppCenter.ReactNative
{
    [ReactModule]
    class AppCenterReactNative
    {
        public AppCenterReactNative()
        {
            _ = AppCenterReactNativeShared.ConfigureAppCenter();
        }

        [ReactMethod("startFromLibrary")]
        public void StartFromLibrary(JSValue service)
        {
            var bindingType = service.AsObject()["bindingType"].AsString();
            var serviceClass = Type.GetType(bindingType);
            Microsoft.AppCenter.AppCenter.Start(serviceClass);
        }

        [ReactMethod("setEnabled")]
        public async void SetEnabled(bool enabled, ReactPromise<bool> promise)
        {
            await Microsoft.AppCenter.AppCenter.SetEnabledAsync(enabled);
            promise.Resolve(enabled);
        }

        [ReactMethod("isEnabled")]
        public async void IsEnabled(ReactPromise<bool> promise)
        {
            promise.Resolve(await Microsoft.AppCenter.AppCenter.IsEnabledAsync());
        }

        [ReactMethod("setLogLevel")]
        public void SetLogLevel(int logLevel)
        {
            Microsoft.AppCenter.AppCenter.LogLevel = (Microsoft.AppCenter.LogLevel)logLevel;
        }

        [ReactMethod("getLogLevel")]
        public void GetLogLevel(ReactPromise<int> promise)
        {
            promise.Resolve((int)Microsoft.AppCenter.AppCenter.LogLevel);
        }

        [ReactMethod("getInstallId")]
        public async void GetInstallId(ReactPromise<string> promise)
        {
            promise.Resolve((await Microsoft.AppCenter.AppCenter.GetInstallIdAsync()).ToString());
        }

        [ReactMethod("setUserId")]
        public void SetUserId(string userId)
        {
            Microsoft.AppCenter.AppCenter.SetUserId(userId);
        }

        [ReactMethod("setCustomProperties")]
        public void SetCustomProperties(JSValue properties)
        {
            CustomProperties customProperties = new CustomProperties();
            foreach (var key in properties.AsObject().Keys)
            {
                var valueObject = properties[key];
                var type = valueObject["type"];
                var value = valueObject["value"];
                switch (type.AsString())
                {
                    case "string":
                        customProperties.Set(key, value.AsString());
                        break;
                    case "number":
                        customProperties.Set(key, value.AsDouble());
                        break;
                    case "boolean":
                        customProperties.Set(key, value.AsBoolean());
                        break;
                    case "date-time":
                        customProperties.Set(key, DateTimeOffset.FromUnixTimeMilliseconds(value.AsInt64()).UtcDateTime);
                        break;
                    case "clear":
                        customProperties.Clear(key);
                        break;
                }
            }
            Microsoft.AppCenter.AppCenter.SetCustomProperties(customProperties);
        }
    }
}
