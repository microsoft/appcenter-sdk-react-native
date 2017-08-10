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

    // typedef bool (^MSUserConfirmationHandler)(NSArray<MSErrorReport *> * _Nonnull);
    delegate bool MSUserConfirmationHandler(MSErrorReport[] arg0);

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

        //+(void)disableMachExceptionHandler;
        [Static]
        [Export("disableMachExceptionHandler")]
        void DisableMachExceptionHandler();
    }

    // @protocol MSCrashesDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSCrashesDelegate
    {
        // @optional -(BOOL)crashes:(MSCrashes *)crashes shouldProcessErrorReport:(MSErrorReport *)errorReport;
        [Export("crashes:shouldProcessErrorReport:")]
        bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport msReport);

        // @optional - (NSArray<MSErrorAttachmentLog *> *)attachmentsWithCrashes:(MSCrashes *)crashes forErrorReport:(MSErrorReport *)errorReport;
        [Export("attachmentsWithCrashes:forErrorReport:")]
        NSArray AttachmentsWithCrashes(MSCrashes crashes, MSErrorReport msReport);

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

    // @interface MSErrorAttachmentLog : NSObject
    [BaseType(typeof(NSObject))]
    interface MSErrorAttachmentLog
    {
        // + (MSErrorAttachmentLog *)attachmentWithText:(NSString *)text filename:(NSString *)filename;
        [Static]
        [Export("attachmentWithText:filename:")]
        MSErrorAttachmentLog AttachmentWithText(string text, [NullAllowed] string fileName);

        // + (MSErrorAttachmentLog *)attachmentWithBinary:(NSData *)data filename:(NSString*)filename contentType:(NSString*)contentType;
        [Static]
        [Export("attachmentWithBinary:filename:contentType:")]
        MSErrorAttachmentLog AttachmentWithBinaryData(NSData data, [NullAllowed] string filename, string contentType);
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

        // @property (nonatomic) NSString * _Nullable stackTrace;
        [NullAllowed, Export("stackTrace")]
        string StackTrace { get; set; }

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


    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSCrashHandlerSetupDelegate
    {
        //- (void) willSetUpCrashHandlers;
        [Export("willSetUpCrashHandlers")]
        void WillSetUpCrashHandlers();

        //- (void) didSetUpCrashHandlers;
        [Export("didSetUpCrashHandlers")]
        void DidSetUpCrashHandlers();

        //- (BOOL) shouldEnableUncaughtExceptionHandler;
        [Export("shouldEnableUncaughtExceptionHandler")]
        bool ShouldEnableUncaughtExceptionHandler();
    }

    // @interface MSWrapperExceptionManager : NSObject
    [BaseType(typeof(NSObject))]
    interface MSWrapperExceptionManager
    {
        //+ (void) saveWrapperException:(MSWrapperException*) wrapperException;
        [Static]
        [Export("saveWrapperException:")]
        void SaveWrapperException(MSWrapperException wrapperException);

        //+ (MSWrapperException*) loadWrapperExceptionWithUUID:(NSString*) uuid;
        [Static]
        [Export("loadWrapperExceptionWithUUIDString:")]
        MSWrapperException LoadWrapperExceptionWithUUID(string uuidString);
    }

    [BaseType(typeof(NSObject))]
    interface MSWrapperException
    {
        //@property(nonatomic, strong) MSException* exception;
        [Export("modelException")]
        MSException Exception { get; set; }

        //@property(nonatomic, strong) NSData* exceptionData;
        [Export("exceptionData")]
        NSData ExceptionData { get; set; }

        [Export("processId")]
        NSNumber ProcessId { get; set; }
    }

    [BaseType(typeof(NSObject))]
    interface MSWrapperCrashesHelper
    {
        //+ (void) setCrashHandlerSetupDelegate:(id<MSCrashHandlerSetupDelegate>) delegate;
        [Static]
        [Export("setCrashHandlerSetupDelegate:")]
        void SetCrashHandlerSetupDelegate(MSCrashHandlerSetupDelegate del);
    }
}
