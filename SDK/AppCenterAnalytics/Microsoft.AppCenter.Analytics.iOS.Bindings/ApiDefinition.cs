// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;

namespace Microsoft.AppCenter.Analytics.iOS.Bindings
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
        void TrackEvent([NullAllowed] string eventName);

        // +(void)trackEvent:(NSString *)eventName withProperties:(NSDictionary *)properties;
        [Static]
        [Export("trackEvent:withProperties:")]
        void TrackEvent([NullAllowed] string eventName, [NullAllowed] NSDictionary properties);

        // +(void)setDelegate:(id<MSAnalyticsDelegate> _Nullable)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] MSAnalyticsDelegate analyticsDelegate);

        // + (void)resetSharedInstance
        [Static]
        [Export("resetSharedInstance")]
        void ResetSharedInstance();

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

    // @protocol MSAnalyticsDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSAnalyticsDelegate
    {
        //@optional - (void)analytics:(MSAnalytics*)analytics willSendEventLog:(MSEventLog*)eventLog;
        [Export("analytics:willSendEventLog:")]
        void WillSendEventLog(MSAnalytics analytics, MSEventLog eventLog);

        //@optional - (void)analytics:(MSAnalytics*)analytics didSucceedSendingEventLog:(MSEventLog*)eventLog;
        [Export("analytics:didSucceedSendingEventLog:")]
        void DidSucceedSendingEventLog(MSAnalytics analytics, MSEventLog eventLog);

        //@optional - (void)analytics:(MSAnalytics*)analytics didFailSendingEventLog:(MSEventLog*)eventLog withError:(NSError*)error;
        [Export("analytics:didFailSendingEventLog:withError:")]
        void DidFailSendingEventLog(MSAnalytics analytics, MSEventLog eventLog, NSError error);

        ////@optional - (void)analytics:(MSAnalytics*)analytics willSendPageLog:(MSPageLog*)pageLog;
        //[Export("analytics:willSendPageLog:")]
        //void WillSendPageLog(MSAnalytics analytics, MSPageLog pageLog);

        ////@optional - (void)analytics:(MSAnalytics*)analytics didSucceedSendingPageLog:(MSPageLog*)pageLog;
        //[Export("analytics:didSucceedSendingPageLog:")]
        //void DidSucceedSendingPageLog(MSAnalytics analytics, MSPageLog pageLog);

        ////@optional - (void)analytics:(MSAnalytics*)analytics didFailSendingPageLog:(MSPageLog*)pageLog withError:(NSError*)error;
        //[Export("analytics:didFailSendingPageLog:withError:")]
        //void DidFailSendingPageLog(MSAnalytics analytics, MSPageLog pageLog, NSError error);
    }

    // @interface MSLogWithProperties : MSAbstractLog
    [BaseType(typeof(NSObject))]
    interface MSLogWithProperties
    {
        //@property(nonatomic) NSDictionary<NSString*, NSString*>* properties;
        [Export("properties")]
        NSDictionary<NSString, NSString> Properties { get; set; }
    }

    //@interface MSEventLog : MSLogWithProperties
    [BaseType(typeof(MSLogWithProperties))]
    interface MSEventLog : MSLogWithProperties
    {
        //@property(nonatomic) NSString *eventId;
        [Export("eventId")]
        string EventId { get; set; }

        //@property(nonatomic) NSString *name;
        [Export("name")]
        string Name { get; set; }
    }

    //@interface MSPageLog : MSLogWithProperties
    [BaseType(typeof(MSLogWithProperties))]
    interface MSPageLog : MSLogWithProperties
    {
        //@property(nonatomic) NSString *name;
        [Export("name")]
        string Name { get; set; }
    }
}
