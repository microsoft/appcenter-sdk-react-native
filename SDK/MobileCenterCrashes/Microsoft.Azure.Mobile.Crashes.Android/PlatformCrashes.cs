using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Azure.Mobile.Crashes.Model;
using Java.Lang;

namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidCrashes = Com.Microsoft.Azure.Mobile.Crashes.AndroidCrashes;
    using AndroidExceptionDataManager = Com.Microsoft.Azure.Mobile.Crashes.WrapperSdkExceptionManager;
    using AndroidICrashListener = Com.Microsoft.Azure.Mobile.Crashes.ICrashesListener;
    using ModelException = Com.Microsoft.Azure.Mobile.Crashes.Ingestion.Models.Exception;
    using ModelStackFrame = Com.Microsoft.Azure.Mobile.Crashes.Ingestion.Models.StackFrame;
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

        //public override void TrackException(Exception exception)
        //{
        //    AndroidCrashes.Instance.TrackException(GenerateModelException(exception));
        //}

        private AndroidICrashListener _crashListener;

        /// <summary>
        /// Empty model stack frame used for comparison to optimize JSON payload.
        /// </summary>
        private static readonly ModelStackFrame EmptyModelFrame = new ModelStackFrame();

        static PlatformCrashes()
        {
            MobileCenterLog.Info(Crashes.LogTag, "Set up Xamarin crash handler.");
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
            MobileCenterLog.Error(Crashes.LogTag, "Unhandled Exception:", exception);
            if (!(exception is Java.Lang.Exception))
            {
                var modelException = GenerateModelException(exception);
                byte[] rawException = CrashesUtils.SerializeException(exception);
                AndroidExceptionDataManager.SaveWrapperException(Thread.CurrentThread(), modelException, rawException);
            }
        }

#pragma warning disable XS0001 // Find usages of mono todo items

        /// <summary>
        /// Generate structured data for a dotnet exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns>Structured data for the exception.</returns>
        private static ModelException GenerateModelException(Exception exception)
        {
            var modelException = new ModelException
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Frames = GenerateModelStackFrames(new StackTrace(exception, true)),
                WrapperSdkName = WrapperSdk.Name
            };
            var aggregateException = exception as AggregateException;
            if (aggregateException?.InnerExceptions != null)
            {
                modelException.InnerExceptions = new List<ModelException>();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    modelException.InnerExceptions.Add(GenerateModelException(innerException));
                }
            }
            else if (exception.InnerException != null)
            {
                modelException.InnerExceptions = new List<ModelException> { GenerateModelException(exception.InnerException) };
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
                    LineNumber = frame.GetFileLineNumber() != 0 ? new Java.Lang.Integer(frame.GetFileLineNumber()) : null
                }).Where(modelFrame => !modelFrame.Equals(EmptyModelFrame)));
            }
            return modelFrames;
        }
#pragma warning restore XS0001 // Find usages of mono todo items
    }
}