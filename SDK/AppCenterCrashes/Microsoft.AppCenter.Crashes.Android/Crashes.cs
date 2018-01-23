using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Crashes;
using Com.Microsoft.Appcenter.Crashes.Model;
using Java.Lang;
using Java.Util;

namespace Microsoft.AppCenter.Crashes
{
    #pragma warning disable XA0001 // Find issues with Android API usage, the API level check does not work in libs.
    using ModelException = Com.Microsoft.Appcenter.Crashes.Ingestion.Models.Exception;
    using ModelStackFrame = Com.Microsoft.Appcenter.Crashes.Ingestion.Models.StackFrame;
    using Exception = System.Exception;

    public partial class Crashes
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidCrashes);

        static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
            int androidUserConfirmation;
            switch (confirmation)
            {
                case UserConfirmation.Send:
                    androidUserConfirmation = AndroidCrashes.Send;
                    break;
                case UserConfirmation.DontSend:
                    androidUserConfirmation = AndroidCrashes.DontSend;
                    break;
                case UserConfirmation.AlwaysSend:
                    androidUserConfirmation = AndroidCrashes.AlwaysSend;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmation), confirmation, null);
            }
            AndroidCrashes.NotifyUserConfirmation(androidUserConfirmation);
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidCrashes.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidCrashes.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            var future = AndroidCrashes.HasCrashedInLastSession();
            return Task.Run(() => (bool)future.Get());
        }

        static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            var future = AndroidCrashes.LastSessionCrashReport;
            return Task.Run(() =>
            {
                var androidErrorReport = future.Get() as AndroidErrorReport;
                if (androidErrorReport == null)
                    return null;
                return ErrorReportCache.GetErrorReport(androidErrorReport);
            });
        }

        static void PlatformTrackError(Exception exception, IDictionary<string, string> properties)
        {
            WrapperSdkExceptionManager.TrackException(GenerateModelException(exception, false), properties);
        }

        // Empty model stack frame used for comparison to optimize JSON payload.
        static readonly ModelStackFrame EmptyModelFrame = new ModelStackFrame();

        static Crashes()
        {
            AppCenterLog.Info(LogTag, "Set up Xamarin crash handler.");
            AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledException;
            AndroidCrashes.SetListener(new AndroidCrashListener());
        }

        static void OnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            var exception = e.Exception;
            AppCenterLog.Error(LogTag, "Unhandled Exception:", exception);
            var javaThrowable = exception as Throwable;
            var modelException = GenerateModelException(exception, true);
            byte[] rawException = javaThrowable == null ? CrashesUtils.SerializeException(exception) : null;
            WrapperSdkExceptionManager.SaveWrapperException(Thread.CurrentThread(), javaThrowable, modelException, rawException);
        }

#pragma warning disable XS0001 // Find usages of mono todo items

        // Generate structured data for a dotnet exception.
        static ModelException GenerateModelException(Exception exception, bool structuredFrames)
        {
            var modelException = new ModelException
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Frames = structuredFrames ? GenerateModelStackFrames(new StackTrace(exception, true)) : null,
                WrapperSdkName = WrapperSdk.Name
            };
            var aggregateException = exception as AggregateException;
            if (aggregateException?.InnerExceptions != null)
            {
                modelException.InnerExceptions = new List<ModelException>();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    modelException.InnerExceptions.Add(GenerateModelException(innerException, structuredFrames));
                }
            }
            else if (exception.InnerException != null)
            {
                modelException.InnerExceptions = new List<ModelException>
                {
                    GenerateModelException(exception.InnerException, structuredFrames)
                };
            }
            return modelException;
        }

        static IList<ModelStackFrame> GenerateModelStackFrames(StackTrace stackTrace)
        {
            var modelFrames = new List<ModelStackFrame>();
            var frames = stackTrace.GetFrames();
            if (frames != null)
            {
                modelFrames.AddRange(frames.Select(frame => new ModelStackFrame
                {
                    ClassName = frame.GetMethod()?.DeclaringType?.FullName,
                    MethodName = frame.GetMethod()?.Name,
                    FileName = frame.GetFileName(),
                    LineNumber = frame.GetFileLineNumber() != 0 ? new Integer(frame.GetFileLineNumber()) : null
                }).Where(modelFrame => !modelFrame.Equals(EmptyModelFrame)));
            }
            return modelFrames;
        }
#pragma warning restore XS0001 // Find usages of mono todo items


        /* Bridge between C# events/callbacks and Java listeners. */
        class AndroidCrashListener : Java.Lang.Object, ICrashesListener
        {
#pragma warning disable RECS0146 // Member hides static member from outer class
            public IIterable GetErrorAttachments(AndroidErrorReport androidReport)
#pragma warning restore RECS0146 // Member hides static member from outer class
            {
                if (Crashes.GetErrorAttachments == null)
                {
                    return null;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var attachments = Crashes.GetErrorAttachments(report);
                if (attachments != null)
                {
                    var attachmentList = new ArrayList();
                    foreach (var attachment in attachments)
                    {
                        /* Let Java SDK warn against null. */
                        attachmentList.Add(attachment?.internalAttachment);
                    }
                    return attachmentList;
                }
                return null;
            }

            public void OnBeforeSending(AndroidErrorReport androidReport)
            {
                if (SendingErrorReport == null)
                {
                    return;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var e = new SendingErrorReportEventArgs
                {
                    Report = report
                };
                SendingErrorReport(null, e);
            }

            public void OnSendingFailed(AndroidErrorReport androidReport, Java.Lang.Exception exception)
            {
                if (FailedToSendErrorReport == null)
                {
                    return;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var e = new FailedToSendErrorReportEventArgs
                {
                    Report = report,
                    Exception = exception
                };
                FailedToSendErrorReport(null, e);
            }

            public void OnSendingSucceeded(AndroidErrorReport androidReport)
            {
                if (SentErrorReport == null)
                {
                    return;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var e = new SentErrorReportEventArgs
                {
                    Report = report
                };
                SentErrorReport(null, e);
            }

#pragma warning disable RECS0146 // Member hides static member from outer class
            public bool ShouldAwaitUserConfirmation()
#pragma warning restore RECS0146 // Member hides static member from outer class
            {
                if (Crashes.ShouldAwaitUserConfirmation != null)
                {
                    return Crashes.ShouldAwaitUserConfirmation();
                }
                return false;
            }

            public bool ShouldProcess(AndroidErrorReport androidReport)
            {
                if (ShouldProcessErrorReport == null)
                {
                    return true;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                return ShouldProcessErrorReport(report);
            }
        }
    }
}