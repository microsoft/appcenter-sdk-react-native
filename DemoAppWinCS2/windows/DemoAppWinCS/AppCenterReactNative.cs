using Microsoft.ReactNative.Managed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AppCenter;
using System.Diagnostics;

namespace DemoAppWinCS
{
    [ReactModule]
    class AppCenterReactNative
    {
        [ReactMethod("startFromLibrary")]
        public void StartFromLibrary(JSValue service)
        {
            var bindingType = service.AsObject()["bindingType"].AsString();
            var serviceClass = Type.GetType(bindingType);
            AppCenter.Start(serviceClass);
        }

        [ReactMethod("setEnabled")]
        public async void SetEnabled(bool enabled, ReactPromise<bool> promise)
        {
            await AppCenter.SetEnabledAsync(enabled);
            promise.Resolve(enabled);
        }

        [ReactMethod("isEnabled")]
        public async void IsEnabled(ReactPromise<bool> promise)
        {
            promise.Resolve(await AppCenter.IsEnabledAsync());
        }

        [ReactMethod("setLogLevel")]
        public void SetLogLevel(int logLevel)
        {
            AppCenter.LogLevel = (Microsoft.AppCenter.LogLevel)logLevel;
        }

        [ReactMethod("getLogLevel")]
        public void GetLogLevel(ReactPromise<int> promise)
        {
            promise.Resolve((int)AppCenter.LogLevel);
        }

        [ReactMethod("getInstallId")]
        public async void GetInstallId(ReactPromise<string> promise)
        {
            promise.Resolve((await AppCenter.GetInstallIdAsync()).ToString());
        }

        [ReactMethod("setUserId")]
        public void SetUserId(String userId)
        {
            AppCenter.SetUserId(userId);
        }

        [ReactMethod("setCustomProperties")]
        public void SetCustomProperties(JSValue properties)
        {
            CustomProperties customProperties = new CustomProperties();
            foreach(var key in properties.AsObject().Keys)
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
            AppCenter.SetCustomProperties(customProperties);
        }
    }
}
