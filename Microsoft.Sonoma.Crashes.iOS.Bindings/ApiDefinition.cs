using System;
using Foundation;
using ObjCRuntime;

namespace Microsoft.Azure.Mobile.Crashes.iOS.Bindings
{
	// @interface SNMErrorReport : NSObject
	[BaseType(typeof(NSObject))]
	interface SNMErrorReport
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

		// @property (readonly, nonatomic) SNMDevice * device;
		[Export("device")]
		Microsoft.Azure.Mobile.iOS.Bindings.SNMDevice Device { get; }

		// @property (readonly, assign, nonatomic) NSUInteger appProcessIdentifier;
		[Export("appProcessIdentifier")]
		nuint AppProcessIdentifier { get; }

		// -(BOOL)isAppKill;
		[Export("isAppKill")]
		//[Verify(MethodToProperty)]
		bool IsAppKill { get; }
	}

	// typedef void (^SNMUserConfirmationHandler)(NSArray<SNMErrorReport *> * _Nonnull);
	delegate void SNMUserConfirmationHandler(SNMErrorReport[] arg0);

    // @interface SNMCrashes
    [BaseType(typeof(NSObject))]
    interface SNMCrashes
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

        //(SNMErrorReport * _Nullable)lastSessionCrashReport;
        [Static]
        [NullAllowed, Export("lastSessionCrashReport")]
        SNMErrorReport LastSessionCrashReport { get; }

        //(void)setDelegate:(id<SNMCrashesDelegate> _Nullable)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] SNMCrashesDelegate crashesDelegate);

        //(void)setUserConfirmationHandler:(SNMUserConfirmationHandler _Nullable)userConfirmationHandler;
        [Static]
        [Export("setUserConfirmationHandler:")]
        void SetUserConfirmationHandler([NullAllowed] SNMUserConfirmationHandler userConfirmationHandler);

        //(void)notifyWithUserConfirmation:(SNMUserConfirmation)userConfirmation;
        [Static]
        [Export("notifyWithUserConfirmation:")]
        void NotifyWithUserConfirmation(SNMUserConfirmation userConfirmation);
    }

    // @protocol SNMCrashesDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface SNMCrashesDelegate
    {
        // @optional -(BOOL)crashes:(SNMCrashes *)crashes shouldProcessErrorReport:(SNMErrorReport *)errorReport;
        [Export("crashes:shouldProcessErrorReport:")]
        bool CrashesShouldProcessErrorReport(SNMCrashes crashes, SNMErrorReport errorReport);

        //TODO figure out why this is marked optional - we get a crash if it is not implemented
        // @optional -(SNMErrorAttachment *)attachmentWithCrashes:(SNMCrashes *)crashes forErrorReport:(SNMErrorReport *)errorReport;
        [Export("attachmentWithCrashes:forErrorReport:")]
        SNMErrorAttachment AttachmentWithCrashes(SNMCrashes crashes, SNMErrorReport errorReport);

        // @optional -(void)crashes:(SNMCrashes *)crashes willSendErrorReport:(SNMErrorReport *)errorReport;
        [Export("crashes:willSendErrorReport:")]
        void CrashesWillSendErrorReport(SNMCrashes crashes, SNMErrorReport errorReport);

        // @optional -(void)crashes:(SNMCrashes *)crashes didSucceedSendingErrorReport:(SNMErrorReport *)errorReport;
        [Export("crashes:didSucceedSendingErrorReport:")]
        void CrashesDidSucceedSendingErrorReport(SNMCrashes crashes, SNMErrorReport errorReport);

        // @optional -(void)crashes:(SNMCrashes *)crashes didFailSendingErrorReport:(SNMErrorReport *)errorReport withError:(NSError *)error;
        [Export("crashes:didFailSendingErrorReport:withError:")]
        void CrashesDidFailSendingErrorReport(SNMCrashes crashes, SNMErrorReport errorReport, NSError error);
    }

    // @interface SNMErrorAttachment : NSObject
    [BaseType(typeof(NSObject))]
    interface SNMErrorAttachment
    {
        // @property (nonatomic) NSString * _Nullable textAttachment;
        [NullAllowed, Export("textAttachment")]
        string TextAttachment { get; set; }

        // @property (nonatomic) SNMErrorBinaryAttachment * _Nullable binaryAttachment;
        [NullAllowed, Export("binaryAttachment", ArgumentSemantic.Assign)]
        SNMErrorBinaryAttachment BinaryAttachment { get; set; }

        // -(BOOL)isEqual:(SNMErrorAttachment * _Nullable)attachment;
        [Export("isEqual:")]
        bool IsEqual([NullAllowed] SNMErrorAttachment attachment);

        // +(SNMErrorAttachment * _Nonnull)attachmentWithText:(NSString * _Nonnull)text;
        [Static]
        [Export("attachmentWithText:")]
        SNMErrorAttachment AttachmentWithText(string text);

        // +(SNMErrorAttachment * _Nonnull)attachmentWithBinaryData:(NSData * _Nonnull)data filename:(NSString * _Nullable)filename mimeType:(NSString * _Nonnull)mimeType;
        [Static]
        [Export("attachmentWithBinaryData:filename:mimeType:")]
        SNMErrorAttachment AttachmentWithBinaryData(NSData data, [NullAllowed] string filename, string mimeType);

        // +(SNMErrorAttachment * _Nonnull)attachmentWithText:(NSString * _Nonnull)text andBinaryData:(NSData * _Nonnull)data filename:(NSString * _Nullable)filename mimeType:(NSString * _Nonnull)mimeType;
        [Static]
        [Export("attachmentWithText:andBinaryData:filename:mimeType:")]
        SNMErrorAttachment AttachmentWithText(string text, NSData data, [NullAllowed] string filename, string mimeType);
    }

    // @interface SNMErrorBinaryAttachment : NSObject
    [BaseType(typeof(NSObject))]
    interface SNMErrorBinaryAttachment
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

        // -(BOOL)isEqual:(SNMErrorBinaryAttachment * _Nullable)attachment;
        [Export("isEqual:")]
        bool IsEqual([NullAllowed] SNMErrorBinaryAttachment attachment);

        // -(instancetype _Nonnull)initWithFileName:(NSString * _Nullable)fileName attachmentData:(NSData * _Nonnull)data contentType:(NSString * _Nonnull)contentType;
        [Export("initWithFileName:attachmentData:contentType:")]
        IntPtr Constructor([NullAllowed] string fileName, NSData data, string contentType);
    }
}
