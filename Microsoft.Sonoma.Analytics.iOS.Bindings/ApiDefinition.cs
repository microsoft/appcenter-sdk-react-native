using Foundation;

namespace Microsoft.Sonoma.Analytics.iOS.Bindings
{
	// @interface SNMAnalytics : SNMFeature
	[BaseType(typeof(Core.iOS.Bindings.SNMFeatureAbstract))]
	interface SNMAnalytics
	{
		// +(void)trackEvent:(NSString *)eventName;
		[Static]
		[Export("trackEvent:")]
		void TrackEvent(string eventName);

		// +(void)trackEvent:(NSString *)eventName withProperties:(NSDictionary *)properties;
		[Static]
		[Export("trackEvent:withProperties:")]
		void TrackEvent(string eventName, NSDictionary properties);

		// +(void)trackPage:(NSString *)pageName;
		[Static]
		[Export("trackPage:")]
		void TrackPage(string pageName);

		// +(void)trackPage:(NSString *)pageName withProperties:(NSDictionary *)properties;
		[Static]
		[Export("trackPage:withProperties:")]
		void TrackPage(string pageName, NSDictionary properties);

		// +(void)setAutoPageTrackingEnabled:(BOOL)isEnabled;
		[Static]
		[Export("setAutoPageTrackingEnabled:")]
		void SetAutoPageTrackingEnabled(bool isEnabled);

		// +(BOOL)isAutoPageTrackingEnabled;
		[Static]
		[Export("isAutoPageTrackingEnabled")]
		bool IsAutoPageTrackingEnabled();

	}
}