// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using ObjCRuntime;
using System;

namespace Microsoft.AppCenter.iOS.Bindings
{
    interface IMSService { }

    // typedef NSString * (^MSLogMessageProvider)();
    delegate string MSLogMessageProvider();

    // typedef void (^MSLogHandler)(MSLogMessageProvider, MSLogLevel, const char *, const char *, uint);
    unsafe delegate void MSLogHandler(MSLogMessageProvider arg0, MSLogLevel arg1, IntPtr arg2, IntPtr arg3, uint arg4);
    //Note: Objective Sharpie tried to bind the above as:
    //  unsafe delegate void MSLogHandler(MSLogMessageProvider arg0, MSLogLevel arg1, string arg2, sbyte* arg3, sbyte* arg4, uint arg5);
    //But trying to use it as given gave an error.

    // @interface MSWrapperSdk : NSObject
    [BaseType(typeof(NSObject))]
    interface MSWrapperSdk
    {
        // @property (readonly, nonatomic) NSString * wrapperSdkVersion;
        [Export("wrapperSdkVersion")]
        string WrapperSdkVersion { get; }

        // @property (readonly, nonatomic) NSString * wrapperSdkName;
        [Export("wrapperSdkName")]
        string WrapperSdkName { get; }

        // @property (readonly, nonatomic) NSString * wrapperRuntimeVersion;
        [Export("wrapperRuntimeVersion")]
        string WrapperRuntimeVersion { get; }

        // @property (readonly, nonatomic) NSString * liveUpdateReleaseLabel;
        [Export("liveUpdateReleaseLabel")]
        string LiveUpdateReleaseLabel { get; }

        // @property (readonly, nonatomic) NSString * liveUpdateDeploymentKey;
        [Export("liveUpdateDeploymentKey")]
        string LiveUpdateDeploymentKey { get; }

        // @property (readonly, nonatomic) NSString * liveUpdatePackageHash;
        [Export("liveUpdatePackageHash")]
        string LiveUpdatePackageHash { get; }

        // -(BOOL)isEqual:(MSWrapperSdk *)wrapperSdk;
        [Export("isEqual:")]
        bool IsEqual(MSWrapperSdk wrapperSdk);

        // initWithWrapperSdkVersion:(NSString *)wrapperSdkVersion wrapperSdkName:(NSString *)wrapperSdkName wrapperRuntimeVersion:(NSString*)wrapperRuntimeVersion liveUpdateReleaseLabel:(NSString*)liveUpdateReleaseLabel liveUpdateDeploymentKey:(NSString*)liveUpdateDeploymentKey liveUpdatePackageHash:(NSString*)liveUpdatePackageHash;
        [Export("initWithWrapperSdkVersion:wrapperSdkName:wrapperRuntimeVersion:liveUpdateReleaseLabel:liveUpdateDeploymentKey:liveUpdatePackageHash:")]
        IntPtr Constructor([NullAllowed] string wrapperSdkVersion, [NullAllowed] string wrapperSdkName, [NullAllowed] string wrapperRuntimeVersion, [NullAllowed] string liveUpdateReleaseLabel, [NullAllowed] string liveUpdateDeploymentKey, [NullAllowed] string liveUpdatePackageHash);
    }

    // @interface MSDevice : MSWrapperSdk
    [BaseType(typeof(MSWrapperSdk))]
    interface MSDevice
    {
        // @property (readonly, nonatomic) NSString * sdkName;
        [Export("sdkName")]
        string SdkName { get; }

        // @property (readonly, nonatomic) NSString * sdkVersion;
        [Export("sdkVersion")]
        string SdkVersion { get; }

        // @property (readonly, nonatomic) NSString * model;
        [Export("model")]
        string Model { get; }

        // @property (readonly, nonatomic) NSString * oemName;
        [Export("oemName")]
        string OemName { get; }

        // @property (readonly, nonatomic) NSString * osName;
        [Export("osName")]
        string OsName { get; }

        // @property (readonly, nonatomic) NSString * osVersion;
        [Export("osVersion")]
        string OsVersion { get; }

        // @property (readonly, nonatomic) NSString * osBuild;
        [Export("osBuild")]
        string OsBuild { get; }

        // @property (readonly, nonatomic) NSNumber * osApiLevel;
        [Export("osApiLevel")]
        NSNumber OsApiLevel { get; }

        // @property (readonly, nonatomic) NSString * locale;
        [Export("locale")]
        string Locale { get; }

        // @property (readonly, nonatomic) NSNumber * timeZoneOffset;
        [Export("timeZoneOffset")]
        NSNumber TimeZoneOffset { get; }

        // @property (readonly, nonatomic) NSString * screenSize;
        [Export("screenSize")]
        string ScreenSize { get; }

        // @property (readonly, nonatomic) NSString * appVersion;
        [Export("appVersion")]
        string AppVersion { get; }

        // @property (readonly, nonatomic) NSString * carrierName;
        [Export("carrierName")]
        string CarrierName { get; }

        // @property (readonly, nonatomic) NSString * carrierCountry;
        [Export("carrierCountry")]
        string CarrierCountry { get; }

        // @property (readonly, nonatomic) NSString * appBuild;
        [Export("appBuild")]
        string AppBuild { get; }

        // @property (readonly, nonatomic) NSString * appNamespace;
        [Export("appNamespace")]
        string AppNamespace { get; }

        // -(BOOL)isEqual:(MSDevice *)device;
        [Export("isEqual:")]
        bool IsEqual(MSDevice device);
    }

    // @interface MSCustomProperties : NSObject
    [BaseType(typeof(NSObject))]
    interface MSCustomProperties
    {
        // - (instancetype)setString:(NSString *)value forKey:(NSString *)key;
        [Export("setString:forKey:")]
        void Set([NullAllowed] string value, [NullAllowed] string key);

        // - (instancetype)setNumber:(NSNumber *)value forKey:(NSString *)key;
        [Export("setNumber:forKey:")]
        void Set(NSNumber value, [NullAllowed] string key);

        // - (instancetype)setBool:(BOOL)value forKey:(NSString *)key;
        [Export("setBool:forKey:")]
        void Set(bool value, [NullAllowed] string key);

        // - (instancetype)setDate:(NSDate *)value forKey:(NSString *)key;
        [Export("setDate:forKey:")]
        void Set(NSDate value, [NullAllowed] string key);

        // - (instancetype)clearPropertyForKey:(NSString *)key;
        [Export("clearPropertyForKey:")]
        void Clear([NullAllowed] string key);
    }

    // @interface MSAppCenter : NSObject
    [BaseType(typeof(NSObject), Name = "MSAppCenter")]
    interface MSAppCenter
    {
        // +(instancetype)sharedInstance;
        [Static]
        [Export("sharedInstance")]
        MSAppCenter SharedInstance();

        // +(void)resetSharedInstance
        [Static]
        [Export("resetSharedInstance")]
        void ResetSharedInstance();

        // +(void)configureWithAppSecret:(NSString *)appSecret;
        [Static]
        [Export("configureWithAppSecret:")]
        void ConfigureWithAppSecret([NullAllowed] string appSecret);

        // +(void)start:(NSString *)appSecret withServices:(NSArray<Class> *)services;
        [Static]
        [Export("start:withServices:")]
        void Start([NullAllowed] string appSecret, Class[] services);

        // +(void)startService:(Class)service;
        [Static]
        [Export("startService:")]
        void StartService(Class service);

        // +(BOOL)isConfigured;
        [Static]
        [Export("isConfigured")]
        bool IsConfigured();

        // +(void)setLogUrl:(NSString *)setLogUrl;
        [Static]
        [Export("setLogUrl:")]
        void SetLogUrl([NullAllowed] string logUrl);

        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // +(MSLogLevel)logLevel;
        // +(void)setLogLevel:(MSLogLevel)logLevel;
        [Static]
        [Export("logLevel")]
        MSLogLevel LogLevel();

        [Static]
        [Export("setLogLevel:")]
        void SetLogLevel(MSLogLevel logLevel);

        // + (void)setUserId:(NSString *)userId;
        [Static]
        [Export("setUserId:")]
        void SetUserId([NullAllowed] string userId);

        // +(void)setLogHandler:(MSLogHandler)logHandler;
        [Static]
        [Export("setLogHandler:")]
        void SetLogHandler(MSLogHandler logHandler);

        // +(void)setWrapperSdk:(MSWrapperSdk *)wrapperSdk;
        [Static]
        [Export("setWrapperSdk:")]
        void SetWrapperSdk(MSWrapperSdk wrapperSdk);

        // +(NSUUID *)installId;
        [Static]
        [Export("installId")]
        NSUuid InstallId();

        // +(BOOL)isDebuggerAttached;
        [Static]
        [Export("isDebuggerAttached")]
        bool IsDebuggerAttached();

        // + (void)setCustomProperties:(MSCustomProperties *)customProperties;
        [Static]
        [Export("setCustomProperties:")]
        void SetCustomProperties([NullAllowed] MSCustomProperties properties);
    }

    // @protocol MSService <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSService
    {
        // @required +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool GetEnabled();

        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool val);
    }

    // @interface MSServiceAbstract : NSObject <MSService>
    [BaseType(typeof(MSService))]
    interface MSServiceAbstract : IMSService
    {
    }

    // @interface MSLogger : NSObject
    [BaseType(typeof(NSObject))]
    interface MSLogger
    {
        // +(void)logMessage:(MSLogMessageProvider)messageProvider level:(MSLogLevel)loglevel tag:(NSString *)tag file:(const char *)file function:(const char *)function line:(uint)line;
        [Static]
        [Export("logMessage:level:tag:file:function:line:")]
        unsafe void LogMessage(MSLogMessageProvider messageProvider, MSLogLevel loglevel, string tag, IntPtr file, IntPtr function, uint line);
    }

    // @interface MSWrapperLogger : NSObject
    [BaseType(typeof(NSObject))]
    interface MSWrapperLogger
    {
        // +(void)MSWrapperLog:(MSLogMessageProvider)message tag:(NSString *)tag level:(MSLogLevel)level;
        [Static]
        [Export("MSWrapperLog:tag:level:")]
        void MSWrapperLog(MSLogMessageProvider message, string tag, MSLogLevel level);
    }

    // typedef void (^MSHttpRequestCompletionHandler)(NSData *_Nullable responseBody, NSHTTPURLResponse *_Nullable response, NSError* _Nullable error);
    delegate void MSHttpRequestCompletionHandler([NullAllowed] NSData responseBody, [NullAllowed] NSHttpUrlResponse response, [NullAllowed] NSError error);

    // @protocol MSHttpClientProtocol <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSHttpClientProtocol
    {
        // @property(nonatomic, weak, nullable) id<MSHttpClientDelegate> delegate;
        // Using a C# property, even when using the Wrap/Weak pattern, we get "setDelegate:]: unrecognized selector sent to instance", work around by using a setter method as we don't need the getter
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] MSHttpClientDelegate httpClientDelegate);

        // @required - (void)sendAsync:(NSURL *)url
        //                      method:(NSString*) method
        //                     headers:(nullable NSDictionary<NSString*, NSString*> *)headers
        //                        data:(nullable NSData *)data
        //           completionHandler:(nullable MSHttpRequestCompletionHandler) completionHandler;
        [Export("sendAsync:method:headers:data:completionHandler:")]
        void SendAsync(NSUrl url, NSString method, [NullAllowed] NSDictionary<NSString, NSString> headers, [NullAllowed] NSData data, [NullAllowed] MSHttpRequestCompletionHandler completionHandler);

        // @required - (void) sendAsync:(NSURL*) url
        //                       method:(NSString*) method
        //                      headers:(nullable NSDictionary<NSString*, NSString*> *)headers
        //                         data:(nullable NSData *)data
        //               retryIntervals:(NSArray*) retryIntervals
        //           compressionEnabled:(BOOL) compressionEnabled
        //            completionHandler:(nullable MSHttpRequestCompletionHandler) completionHandler;
        [Export("sendAsync:method:headers:data:retryIntervals:compressionEnabled:completionHandler:")]
        void SendAsync(NSUrl url, NSString method, [NullAllowed] NSDictionary<NSString, NSString> headers, [NullAllowed] NSData data, NSArray retryIntervals, bool compressionEnabled, [NullAllowed] MSHttpRequestCompletionHandler completionHandler);

        // @required - (void)pause;
        [Export("pause")]
        void Pause();

        // @required - (void)resume;
        [Export("resume")]
        void Resume();

        // @required - (void)setEnabled:(BOOL)isEnabled;
        [Export("setEnabled:")]
        void SetEnabled(bool enabled);
    }

    // @protocol MSHttpClientDelegate <NSObject>
    [Protocol]
    [BaseType(typeof(NSObject))]
    interface MSHttpClientDelegate
    {
        // -(void)willSendHTTPRequestToURL:(NSURL *)url withHeaders:(nullable NSDictionary<NSString *, NSString *> *)headers;
        [Export("willSendHTTPRequestToURL:withHeaders:")]
        void WillSendHTTPRequestToURL(NSUrl url, [NullAllowed] NSDictionary<NSString, NSString> headers);
    }

    // @interface MSDependencyConfiguration : NSObject
    [BaseType(typeof(NSObject))]
    interface MSDependencyConfiguration
    {
        // @property(class, nonatomic) id<MSHttpClientProtocol> httpClient;
        [Static]
        [Export("httpClient")]
        MSHttpClientProtocol HttpClient { get; set; }
    }
}
