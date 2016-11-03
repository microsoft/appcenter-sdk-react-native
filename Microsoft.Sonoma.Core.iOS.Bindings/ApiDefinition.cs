using Foundation;
using ObjCRuntime;
using System;

namespace Microsoft.Azure.Mobile.iOS.Bindings
{
	interface IMSService { }

	// typedef NSString * (^MSLogMessageProvider)();
	delegate string MSLogMessageProvider();

	// typedef void (^MSLogHandler)(MSLogMessageProvider, MSLogLevel, const char *, const char *, uint);
	unsafe delegate void MSLogHandler(MSLogMessageProvider arg0, MSLogLevel arg1, IntPtr arg2, IntPtr arg3, uint arg4);
	//Note: Objective Sharpie tried to bind the above as:
	//	unsafe delegate void MSLogHandler(MSLogMessageProvider arg0, MSLogLevel arg1, string arg2, sbyte* arg3, sbyte* arg4, uint arg5);
	//But trying to use it as given gave an error.

	// @interface MSWrapperSdk : NSObject
	[BaseType (typeof(NSObject))]
	interface MSWrapperSdk
	{
		// @property (readonly, nonatomic) NSString * wrapperSdkVersion;
		[Export("wrapperSdkVersion")]
		string WrapperSdkVersion { get; }

		// @property (readonly, nonatomic) NSString * wrapperSdkName;
		[Export("wrapperSdkName")]
		string WrapperSdkName { get; }

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

		// -(instancetype)initWithWrapperSdkVersion:(NSString *)wrapperSdkVersion wrapperSdkName:(NSString *)wrapperSdkName liveUpdateReleaseLabel:(NSString *)liveUpdateReleaseLabel liveUpdateDeploymentKey:(NSString *)liveUpdateDeploymentKey liveUpdatePackageHash:(NSString *)liveUpdatePackageHash;
		[Export("initWithWrapperSdkVersion:wrapperSdkName:liveUpdateReleaseLabel:liveUpdateDeploymentKey:liveUpdatePackageHash:")]
		IntPtr Constructor(string wrapperSdkVersion, string wrapperSdkName, string liveUpdateReleaseLabel, string liveUpdateDeploymentKey, string liveUpdatePackageHash);
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

	// @interface MSMobileCenter : NSObject
	[BaseType(typeof(NSObject))]
	interface MSMobileCenter
	{
		// +(instancetype)sharedInstance;
		[Static]
		[Export("sharedInstance")]
		MSMobileCenter SharedInstance();

		// +(void)start:(NSString *)appSecret;
		[Static]
		[Export("start:")]
		void Start(string appSecret);

		// +(void)start:(NSString *)appSecret withServices:(NSArray<Class> *)services;
		[Static]
		[Export("start:withServices:")]
		void Start(string appSecret, Class[] services);

		// +(void)startService:(Class)service;
		[Static]
		[Export("startService:")]
		void StartService(Class service);

		// +(BOOL)isInitialized;
		[Static]
		[Export("isInitialized")]
		bool IsInitialized();

		// +(void)setServerUrl:(NSString *)serverUrl;
		[Static]
		[Export("setServerUrl:")]
		void SetServerUrl(string serverUrl);

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

		//TODO this needs to be fixed
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
}