using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ReactNative;
using Microsoft.ReactNative.Managed;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

using Windows.Storage;
using Windows.Data.Json;

namespace DemoAppWinCS
{
	[ReactModule]
	class AppCenterReactNativeAnalytics
	{
		[ReactMethod("setEnabled")]
		public void SetEnabled(bool enabled, ReactPromise<JSValue> promise) {
			Analytics.SetEnabledAsync(enabled);
			promise.Resolve(JSValue.Null);
		}

		[ReactMethod("isEnabled")]
		public void IsEnabled(ReactPromise<bool> promise) {
			promise.Resolve(Analytics.IsEnabledAsync().Result);
		}

		[ReactMethod("trackEvent")]
		public void TrackEvent(string eventName,
						Dictionary<string, string> properties,
						ReactPromise<JSValue> promise) {
			Analytics.TrackEvent(eventName, properties);
			promise.Resolve(JSValue.Null);
		}
	}

}
