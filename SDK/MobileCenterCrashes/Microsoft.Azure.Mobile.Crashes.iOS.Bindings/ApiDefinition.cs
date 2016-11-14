using System;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace Microsoft.Azure.Mobile.Crashes.iOS.Bindings
{
	// @interface MSErrorReport : NSObject
	[BaseType(typeof(NSObject))]
	interface MSErrorReport
	{
		// @property (readonly, nonatomic) NSString * incidentIdentifier;
		[Export("incidentIdentifier")]
		string IncidentIdentifier { get; }

		// @property (readonly, nonatomic) NSString * reporterKey;
		[Export("reporterKey")]
		string ReporterKey { get; }

		// @property (readonly, nonatomic) NSString * signal;
		[Export("signal")]
		string Signal { get; }

		// @property (readonly, nonatomic) NSString * exceptionName;
		[Export("exceptionName")]
		string ExceptionName { get; }

		// @property (readonly, nonatomic) NSString * exceptionReason;
		[Export("exceptionReason")]
		string ExceptionReason { get; }

		// @property (readonly, nonatomic, strong) NSDate * appStartTime;
		[Export("appStartTime", ArgumentSemantic.Strong)]
		NSDate AppStartTime { get; }

		// @property (readonly, nonatomic, strong) NSDate * appErrorTime;
		[Export("appErrorTime", ArgumentSemantic.Strong)]
		NSDate AppErrorTime { get; }

		// @property (readonly, nonatomic) MSDevice * device;
		[Export("device")]
		Microsoft.Azure.Mobile.iOS.Bindings.MSDevice Device { get; }

		// @property (readonly, assign, nonatomic) NSUInteger appProcessIdentifier;
		[Export("appProcessIdentifier")]
		nuint AppProcessIdentifier { get; }

		// -(BOOL)isAppKill;
		[Export("isAppKill")]
		//[Verify(MethodToProperty)]
		bool IsAppKill { get; }
	}

	// typedef void (^MSUserConfirmationHandler)(NSArray<MSErrorReport *> * _Nonnull);
	delegate void MSUserConfirmationHandler(MSErrorReport[] arg0);

    // @interface MSCrashes
    [BaseType(typeof(NSObject))]
    interface MSCrashes
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // +(void)generateTestCrash;
        [Static]
        [Export("generateTestCrash")]
        void GenerateTestCrash();

        //(BOOL)hasCrashedInLastSession;
        [Static]
        [Export("hasCrashedInLastSession")]
        bool HasCrashedInLastSession { get; }

        //(MSErrorReport * _Nullable)lastSessionCrashReport;
        [Static]
        [NullAllowed, Export("lastSessionCrashReport")]
        MSErrorReport LastSessionCrashReport { get; }

        //(void)setDelegate:(id<MSCrashesDelegate> _Nullable)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] MSCrashesDelegate crashesDelegate);

        //(void)setUserConfirmationHandler:(MSUserConfirmationHandler _Nullable)userConfirmationHandler;
        [Static]
        [Export("setUserConfirmationHandler:")]
        void SetUserConfirmationHandler([NullAllowed] MSUserConfirmationHandler userConfirmationHandler);

        //(void)notifyWithUserConfirmation:(MSUserConfirmation)userConfirmation;
        [Static]
        [Export("notifyWithUserConfirmation:")]
        void NotifyWithUserConfirmation(MSUserConfirmation userConfirmation);
    }

    // @protocol MSCrashesDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSCrashesDelegate
    {
        // @optional -(BOOL)crashes:(MSCrashes *)crashes shouldProcessErrorReport:(MSErrorReport *)errorReport;
        [Export("crashes:shouldProcessErrorReport:")]
        bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport msReport);

        // @optional -(MSErrorAttachment *)attachmentWithCrashes:(MSCrashes *)crashes forErrorReport:(MSErrorReport *)errorReport;
        [Export("attachmentWithCrashes:forErrorReport:")]
        MSErrorAttachment AttachmentWithCrashes(MSCrashes crashes, MSErrorReport msReport);

        // @optional -(void)crashes:(MSCrashes *)crashes willSendErrorReport:(MSErrorReport *)errorReport;
        [Export("crashes:willSendErrorReport:")]
        void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport msReport);

        // @optional -(void)crashes:(MSCrashes *)crashes didSucceedSendingErrorReport:(MSErrorReport *)errorReport;
        [Export("crashes:didSucceedSendingErrorReport:")]
        void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport msReport);

        // @optional -(void)crashes:(MSCrashes *)crashes didFailSendingErrorReport:(MSErrorReport *)errorReport withError:(NSError *)error;
        [Export("crashes:didFailSendingErrorReport:withError:")]
        void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport msReport, NSError error);
    }

    // @interface MSErrorAttachment : NSObject
    [BaseType(typeof(NSObject))]
    interface MSErrorAttachment
    {
        // @property (nonatomic) NSString * _Nullable textAttachment;
        [NullAllowed, Export("textAttachment")]
        string TextAttachment { get; set; }

        // @property (nonatomic) MSErrorBinaryAttachment * _Nullable binaryAttachment;
        [NullAllowed, Export("binaryAttachment", ArgumentSemantic.Assign)]
        MSErrorBinaryAttachment BinaryAttachment { get; set; }

        // -(BOOL)isEqual:(MSErrorAttachment * _Nullable)attachment;
        [Export("isEqual:")]
        bool IsEqual([NullAllowed] MSErrorAttachment attachment);

        // +(MSErrorAttachment * _Nonnull)attachmentWithText:(NSString * _Nonnull)text;
        [Static]
        [Export("attachmentWithText:")]
        MSErrorAttachment AttachmentWithText(string text);

        // +(MSErrorAttachment * _Nonnull)attachmentWithBinaryData:(NSData * _Nonnull)data filename:(NSString * _Nullable)filename mimeType:(NSString * _Nonnull)mimeType;
        [Static]
        [Export("attachmentWithBinaryData:filename:mimeType:")]
        MSErrorAttachment AttachmentWithBinaryData(NSData data, [NullAllowed] string filename, string mimeType);

        // +(MSErrorAttachment * _Nonnull)attachmentWithText:(NSString * _Nonnull)text andBinaryData:(NSData * _Nonnull)data filename:(NSString * _Nullable)filename mimeType:(NSString * _Nonnull)mimeType;
        [Static]
        [Export("attachmentWithText:andBinaryData:filename:mimeType:")]
        MSErrorAttachment AttachmentWithText(string text, NSData data, [NullAllowed] string filename, string mimeType);
    }

    // @interface MSErrorBinaryAttachment : NSObject
    [BaseType(typeof(NSObject))]
    interface MSErrorBinaryAttachment
    {
        // @property (readonly, nonatomic) NSString * _Nullable fileName;
        [NullAllowed, Export("fileName")]
        string FileName { get; }

        // @property (readonly, nonatomic) NSData * _Nonnull data;
        [Export("data")]
        NSData Data { get; }

        // @property (readonly, nonatomic) NSString * _Nonnull contentType;
        [Export("contentType")]
        string ContentType { get; }

        // -(BOOL)isEqual:(MSErrorBinaryAttachment * _Nullable)attachment;
        [Export("isEqual:")]
        bool IsEqual([NullAllowed] MSErrorBinaryAttachment attachment);

        // -(instancetype _Nonnull)initWithFileName:(NSString * _Nullable)fileName attachmentData:(NSData * _Nonnull)data contentType:(NSString * _Nonnull)contentType;
        [Export("initWithFileName:attachmentData:contentType:")]
        IntPtr Constructor([NullAllowed] string fileName, NSData data, string contentType);
    }

    // @interface MSException : NSObject
    [BaseType(typeof(NSObject))]
    interface MSException
    {
        // @property (nonatomic) NSString * _Nonnull type;
        [Export("type")]
        string Type { get; set; }

        // @property (nonatomic) NSString * _Nonnull message;
        [Export("message")]
        string Message { get; set; }

        // @property (nonatomic) NSArray<MSStackFrame *> * _Nullable frames;
        [NullAllowed, Export("frames", ArgumentSemantic.Assign)]
        MSStackFrame[] Frames { get; set; }

        // @property (nonatomic) NSArray<MSException *> * _Nullable innerExceptions;
        [NullAllowed, Export("innerExceptions", ArgumentSemantic.Assign)]
        MSException[] InnerExceptions { get; set; }

        // @property (nonatomic) NSString * _Nullable wrapperSdkName;
        [NullAllowed, Export("wrapperSdkName")]
        string WrapperSdkName { get; set; }

        // -(BOOL)isEqual:(MSException * _Nullable)exception;
        [Export("isEqual:")]
        bool IsEqual([NullAllowed] MSException exception);
    }

    // @interface MSStackFrame : NSObject
    [BaseType(typeof(NSObject))]
    interface MSStackFrame
    {
        // @property (nonatomic) NSString * _Nullable address;
        [NullAllowed, Export("address")]
        string Address { get; set; }

        // @property (nonatomic) NSString * _Nullable code;
        [NullAllowed, Export("code")]
        string Code { get; set; }

        // @property (nonatomic) NSString * _Nullable className;
        [NullAllowed, Export("className")]
        string ClassName { get; set; }

        // @property (nonatomic) NSString * _Nullable methodName;
        [NullAllowed, Export("methodName")]
        string MethodName { get; set; }

        // @property (nonatomic) NSNumber * _Nullable lineNumber;
        [NullAllowed, Export("lineNumber", ArgumentSemantic.Assign)]
        NSNumber LineNumber { get; set; }

        // @property (nonatomic) NSString * _Nullable fileName;
        [NullAllowed, Export("fileName")]
        string FileName { get; set; }

        // -(BOOL)isEqual:(MSStackFrame * _Nullable)frame;
        [Export("isEqual:")]
        bool IsEqual([NullAllowed] MSStackFrame frame);
    }

    // @interface MSWrapperExceptionManager : NSObject
    [BaseType(typeof(NSObject))]
    interface MSWrapperExceptionManager
    {
        // +(void)setWrapperException:(MSException *)exception;
        [Static]
        [Export("setWrapperException:")]
        void SetWrapperException(MSException exception);

        [Static]
        [Export("setWrapperExceptionData:")]
        void SetWrapperExceptionData(NSData wrapperExceptionData);

        [Static]
        [Export("loadWrapperExceptionDataWithUUIDString:")]
        NSData LoadWrapperExceptionData(string uuidString);
    }
}
