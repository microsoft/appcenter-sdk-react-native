using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Crashes;
using Com.Microsoft.Appcenter.Crashes.Model;
using Java.Lang;

namespace Microsoft.AppCenter.Crashes
{
    using ModelException = Com.Microsoft.Appcenter.Crashes.Ingestion.Models.Exception;
    using ModelStackFrame = Com.Microsoft.Appcenter.Crashes.Ingestion.Models.StackFrame;
    using Exception = System.Exception;

    class PlatformCrashes : PlatformCrashesBase
    {
        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }
        public override SentErrorReportEventHandler SentErrorReport { get; set; }
        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        public override GetErrorAttachmentsCallback GetErrorAttachments { get; set; }
        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
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

        public override Type BindingType => typeof(AndroidCrashes);

        public override Task<bool> IsEnabledAsync()
        {
            var future = AndroidCrashes.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        public override Task SetEnabledAsync(bool enabled)
        {
            var future = AndroidCrashes.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        public override Task<bool> HasCrashedInLastSessionAsync()
        {
            var future = AndroidCrashes.HasCrashedInLastSession();
            return Task.Run(() => (bool)future.Get());
        }

        public override Task<ErrorReport> GetLastSessionCrashReportAsync()
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

        public override void TrackException(Exception exception)
        {
            WrapperSdkExceptionManager.TrackException(GenerateModelException(exception, false));
        }

        private ICrashesListener _crashListener;

        /// <summary>
        /// Empty model stack frame used for comparison to optimize JSON payload.
        /// </summary>
        private static readonly ModelStackFrame EmptyModelFrame = new ModelStackFrame();

        static PlatformCrashes()
        {
            AppCenterLog.Info(Crashes.LogTag, "Set up Xamarin crash handler.");
            AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledException;
        }

        public PlatformCrashes()
        {
            _crashListener = new AndroidCrashListener(this);
            AndroidCrashes.SetListener(_crashListener);
        }

        private static void OnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            var exception = e.Exception;
            AppCenterLog.Error(Crashes.LogTag, "Unhandled Exception:", exception);
            if (!(exception is Java.Lang.Exception))
            {
                var modelException = GenerateModelException(exception, true);
                byte[] rawException = CrashesUtils.SerializeException(exception);
                WrapperSdkExceptionManager.SaveWrapperException(Thread.CurrentThread(), modelException, rawException);
            }
        }

#pragma warning disable XS0001 // Find usages of mono todo items

        // Generate structured data for a dotnet exception.
        private static ModelException GenerateModelException(Exception exception, bool structuredFrames)
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

        private static IList<ModelStackFrame> GenerateModelStackFrames(StackTrace stackTrace)
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
    }
}