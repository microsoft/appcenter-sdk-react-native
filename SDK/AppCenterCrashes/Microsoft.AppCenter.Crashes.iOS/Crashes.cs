using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Crashes.iOS.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Crashes bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(MSCrashes);

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSCrashes.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSCrashes.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            return Task.FromResult(MSCrashes.HasCrashedInLastSession);
        }

        static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Task.Run(() =>
            {
                var msReport = MSCrashes.LastSessionCrashReport;
                return (msReport == null) ? null : new ErrorReport(msReport);
            });
        }

        static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
            MSUserConfirmation iosUserConfirmation;
            switch (confirmation)
            {
                case UserConfirmation.Send:
                    iosUserConfirmation = MSUserConfirmation.Send;
                    break;
                case UserConfirmation.DontSend:
                    iosUserConfirmation = MSUserConfirmation.DontSend;
                    break;
                case UserConfirmation.AlwaysSend:
                    iosUserConfirmation = MSUserConfirmation.Always;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmation), confirmation, null);
            }
            MSCrashes.NotifyWithUserConfirmation(iosUserConfirmation);
        }

        static void PlatformTrackError(Exception exception, IDictionary<string, string> properties)
        {
            if (properties != null)
            {
                MSCrashes.TrackModelException(GenerateiOSException(exception, false), StringDictToNSDict(properties));
                return;
            }
            MSCrashes.TrackModelException(GenerateiOSException(exception, false));
        }

        /// <summary>
        /// We keep the reference to avoid it being freed, inlining this object will cause listeners not to be called.
        /// </summary>
        static readonly CrashesDelegate _crashesDelegate = new CrashesDelegate();

        static Crashes()
        {
            /* Perform custom setup around the native SDK's for setting signal handlers */
            MSCrashes.DisableMachExceptionHandler();
            MSWrapperCrashesHelper.SetCrashHandlerSetupDelegate(new CrashesInitializationDelegate());
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            MSCrashes.SetUserConfirmationHandler((reports) =>
                    {
                        if (ShouldAwaitUserConfirmation != null)
                        {
                            return ShouldAwaitUserConfirmation();
                        }
                        return false;
                    });
            MSCrashes.SetDelegate(_crashesDelegate);
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception systemException = e.ExceptionObject as Exception;
            AppCenterLog.Error(LogTag, "Unhandled Exception:", systemException);
            MSException exception = GenerateiOSException(systemException, true);
            byte[] exceptionBytes = CrashesUtils.SerializeException(systemException) ?? new byte[0];
            NSData wrapperExceptionData = NSData.FromArray(exceptionBytes);
            MSWrapperException wrapperException = new MSWrapperException
            {
                Exception = exception,
                ExceptionData = wrapperExceptionData,
                ProcessId = new NSNumber(Process.GetCurrentProcess().Id)
            };
            AppCenterLog.Info(LogTag, "Saving wrapper exception...");
            MSWrapperExceptionManager.SaveWrapperException(wrapperException);
            AppCenterLog.Info(LogTag, "Saved wrapper exception.");
        }

        static MSException GenerateiOSException(Exception exception, bool structuredFrames)
        {
            var msException = new MSException();
            msException.Type = exception.GetType().FullName;
            msException.Message = exception.Message;
            msException.StackTrace = exception.StackTrace;
            msException.Frames = structuredFrames ? GenerateStackFrames(exception) : null;
            msException.WrapperSdkName = WrapperSdk.Name;

            var aggregateException = exception as AggregateException;
            var innerExceptions = new List<MSException>();

            if (aggregateException?.InnerExceptions != null)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    innerExceptions.Add(GenerateiOSException(innerException, structuredFrames));
                }
            }
            else if (exception.InnerException != null)
            {
                innerExceptions.Add(GenerateiOSException(exception.InnerException, structuredFrames));
            }

            msException.InnerExceptions = innerExceptions.Count > 0 ? innerExceptions.ToArray() : null;

            return msException;
        }

#pragma warning disable XS0001 // Find usages of mono todo items

        static MSStackFrame[] GenerateStackFrames(Exception e)
        {
            var trace = new StackTrace(e, true);
            var frameList = new List<MSStackFrame>();

            for (int i = 0; i < trace.FrameCount; ++i)
            {
                StackFrame dotnetFrame = trace.GetFrame(i);
                if (dotnetFrame.GetMethod() == null) continue;
                var msFrame = new MSStackFrame();
                msFrame.Address = null;
                msFrame.Code = null;
                msFrame.MethodName = dotnetFrame.GetMethod().Name;
                msFrame.ClassName = dotnetFrame.GetMethod().DeclaringType?.FullName;
                msFrame.LineNumber = dotnetFrame.GetFileLineNumber() == 0 ? null : (NSNumber)(dotnetFrame.GetFileLineNumber());
                msFrame.FileName = AnonymizePath(dotnetFrame.GetFileName());
                frameList.Add(msFrame);
            }
            return frameList.ToArray();
        }

#pragma warning restore XS0001 // Find usages of mono todo items

        static string AnonymizePath(string path)
        {
            if ((path == null) || (path.Count() == 0) || !path.Contains("/Users/"))
            {
                return path;
            }

            string pattern = "(/Users/[^/]+/)";
            return Regex.Replace(path, pattern, "/Users/USER/");
        }

        // Bridge between .NET events/callbacks and Apple native SDK
        class CrashesDelegate : MSCrashesDelegate
        {
            public override bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport msReport)
            {
                if (ShouldProcessErrorReport == null)
                {
                    return true;
                }
                var report = new ErrorReport(msReport);
                return ShouldProcessErrorReport(report);
            }

            public override NSArray AttachmentsWithCrashes(MSCrashes crashes, MSErrorReport msReport)
            {
                if (GetErrorAttachments == null)
                {
                    return null;
                }
                var report = new ErrorReport(msReport);
                var attachments = GetErrorAttachments(report);
                if (attachments != null)
                {
                    var nsArray = new NSMutableArray();
                    foreach (var attachment in attachments)
                    {
                        if (attachment != null)
                        {
                            nsArray.Add(attachment.internalAttachment);
                        }
                        else
                        {
                            AppCenterLog.Warn(LogTag, "Skipping null ErrorAttachmentLog in Crashes.GetErrorAttachments.");
                        }
                    }
                    return nsArray;
                }
                return null;
            }

            public override void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport msReport)
            {
                if (SendingErrorReport != null)
                {
                    var report = new ErrorReport(msReport);
                    var e = new SendingErrorReportEventArgs
                    {
                        Report = report
                    };
                    SendingErrorReport(null, e);
                }
            }

            public override void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport msReport)
            {
                if (SentErrorReport != null)
                {
                    var report = new ErrorReport(msReport);
                    var e = new SentErrorReportEventArgs
                    {
                        Report = report
                    };
                    SentErrorReport(null, e);
                }

            }

            public override void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport msReport, NSError error)
            {
                if (FailedToSendErrorReport != null)
                {
                    var report = new ErrorReport(msReport);
                    var e = new FailedToSendErrorReportEventArgs
                    {
                        Report = report,
                        Exception = error
                    };
                    FailedToSendErrorReport(null, e);
                }
            }
        }

        private static NSDictionary StringDictToNSDict(IDictionary<string, string> dict)
        {
            return NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }
    }
}
