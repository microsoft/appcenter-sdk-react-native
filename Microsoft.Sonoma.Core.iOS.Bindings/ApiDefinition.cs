using Foundation;
using ObjCRuntime;
using System;

namespace Microsoft.Sonoma.Core.iOS.Bindings
{
	interface ISNMFeature { }

	// typedef NSString * (^SNMLogMessageProvider)();
	delegate string SNMLogMessageProvider();

	// typedef void (^SNMLogHandler)(SNMLogMessageProvider, SNMLogLevel, const char *, const char *, uint);
	unsafe delegate void SNMLogHandler(SNMLogMessageProvider arg0, SNMLogLevel arg1, IntPtr arg2, IntPtr arg3, uint arg4);
	//Note: Objective Sharpie tried to bind the above as:
	//	unsafe delegate void SNMLogHandler(SNMLogMessageProvider arg0, SNMLogLevel arg1, string arg2, sbyte* arg3, sbyte* arg4, uint arg5);
	//But trying to use it as given gave an error.

	// @interface SNMWrapperSdk : NSObject
	[BaseType (typeof(NSObject))]
	interface SNMWrapperSdk
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

		// -(BOOL)isEqual:(SNMWrapperSdk *)wrapperSdk;
		[Export("isEqual:")]
		bool IsEqual(SNMWrapperSdk wrapperSdk);

		// -(instancetype)initWithWrapperSdkVersion:(NSString *)wrapperSdkVersion wrapperSdkName:(NSString *)wrapperSdkName liveUpdateReleaseLabel:(NSString *)liveUpdateReleaseLabel liveUpdateDeploymentKey:(NSString *)liveUpdateDeploymentKey liveUpdatePackageHash:(NSString *)liveUpdatePackageHash;
		[Export("initWithWrapperSdkVersion:wrapperSdkName:liveUpdateReleaseLabel:liveUpdateDeploymentKey:liveUpdatePackageHash:")]
		IntPtr Constructor(string wrapperSdkVersion, string wrapperSdkName, string liveUpdateReleaseLabel, string liveUpdateDeploymentKey, string liveUpdatePackageHash);
	}

	// @interface SNMDevice : SNMWrapperSdk
	[BaseType(typeof(SNMWrapperSdk))]
	interface SNMDevice
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

		// -(BOOL)isEqual:(SNMDevice *)device;
		[Export("isEqual:")]
		bool IsEqual(SNMDevice device);
	}

	// @interface SNMSonoma : NSObject
	[BaseType(typeof(NSObject))]
	interface SNMSonoma
	{
		// +(instancetype)sharedInstance;
		[Static]
		[Export("sharedInstance")]
		SNMSonoma SharedInstance();

		// +(void)start:(NSString *)appSecret;
		[Static]
		[Export("start:")]
		void Start(string appSecret);

		// +(void)start:(NSString *)appSecret withFeatures:(NSArray<Class> *)features;
		[Static]
		[Export("start:withFeatures:")]
		void Start(string appSecret, Class[] features);

		// +(void)startFeature:(Class)feature;
		[Static]
		[Export("startFeature:")]
		void StartFeature(Class feature);

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

		// +(SNMLogLevel)logLevel;
		// +(void)setLogLevel:(SNMLogLevel)logLevel;
		[Static]
		[Export("logLevel")]
		SNMLogLevel LogLevel();

		[Static]
		[Export("setLogLevel:")]
		void SetLogLevel(SNMLogLevel logLevel);

		//TODO this needs to be fixed
		// +(void)setLogHandler:(SNMLogHandler)logHandler;
		[Static]
		[Export("setLogHandler:")]
		void SetLogHandler(SNMLogHandler logHandler);

		// +(void)setWrapperSdk:(SNMWrapperSdk *)wrapperSdk;
		[Static]
		[Export("setWrapperSdk:")]
		void SetWrapperSdk(SNMWrapperSdk wrapperSdk);

		// +(NSUUID *)installId;
		[Static]
		[Export("installId")]
		NSUuid InstallId();

		// +(BOOL)isDebuggerAttached;
		[Static]
		[Export("isDebuggerAttached")]
		bool IsDebuggerAttached();
	}

	// @protocol SNMFeature <NSObject>
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface SNMFeature
	{
		// @required +(BOOL)isEnabled;
		[Static, Abstract]
        [Export("isEnabled")]
        bool Enabled { get; set;}
	}
	// @interface SNMFeatureAbstract : NSObject <SNMFeature>
	[BaseType(typeof(SNMFeature))]
	interface SNMFeatureAbstract : ISNMFeature
	{
	}

	// @interface SNMLogger : NSObject
	[BaseType(typeof(NSObject))]
	interface SNMLogger
	{
		// +(void)logMessage:(SNMLogMessageProvider)messageProvider level:(SNMLogLevel)loglevel tag:(NSString *)tag file:(const char *)file function:(const char *)function line:(uint)line;
		[Static]
		[Export("logMessage:level:tag:file:function:line:")]
		unsafe void LogMessage(SNMLogMessageProvider messageProvider, SNMLogLevel loglevel, string tag, IntPtr file, IntPtr function, uint line);
	}

	// @interface SNMWrapperLogger : NSObject
	[BaseType(typeof(NSObject))]
	interface SNMWrapperLogger
	{
		// +(void)SNMWrapperLog:(SNMLogMessageProvider)message tag:(NSString *)tag level:(SNMLogLevel)level;
		[Static]
		[Export("SNMWrapperLog:tag:level:")]
		void SNMWrapperLog(SNMLogMessageProvider message, string tag, SNMLogLevel level);
	}
}