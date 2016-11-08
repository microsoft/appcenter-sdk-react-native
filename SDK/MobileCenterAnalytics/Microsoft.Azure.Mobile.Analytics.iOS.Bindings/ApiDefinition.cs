using Foundation;

namespace Microsoft.Azure.Mobile.Analytics.iOS.Bindings
{
    // @interface MSAnalytics : MSService
    [BaseType(typeof(NSObject))]
    interface MSAnalytics
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

		// +(void)trackEvent:(NSString *)eventName;
		[Static]
		[Export("trackEvent:")]
		void TrackEvent(string eventName);

		// +(void)trackEvent:(NSString *)eventName withProperties:(NSDictionary *)properties;
		[Static]
		[Export("trackEvent:withProperties:")]
		void TrackEvent(string eventName, NSDictionary properties);

		//// +(void)trackPage:(NSString *)pageName;
		//[Static]
		//[Export("trackPage:")]
		//void TrackPage(string pageName);

		//// +(void)trackPage:(NSString *)pageName withProperties:(NSDictionary *)properties;
		//[Static]
		//[Export("trackPage:withProperties:")]
		//void TrackPage(string pageName, NSDictionary properties);

		//// +(void)setAutoPageTrackingEnabled:(BOOL)isEnabled;
		//[Static]
		//[Export("setAutoPageTrackingEnabled:")]
		//void SetAutoPageTrackingEnabled(bool isEnabled);

		//// +(BOOL)isAutoPageTrackingEnabled;
		//[Static]
		//[Export("isAutoPageTrackingEnabled")]
		//bool IsAutoPageTrackingEnabled();
	}
}