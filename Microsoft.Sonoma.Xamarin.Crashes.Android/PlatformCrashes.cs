using Android.Runtime;
using Android.Util;
using Com.Microsoft.Sonoma.Crashes.Ingestion.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Sonoma.Xamarin.Crashes.Shared;

namespace Microsoft.Sonoma.Xamarin.Crashes
{
    using AndroidCrashes = Com.Microsoft.Sonoma.Crashes.Crashes;
    using Exception = System.Exception;
    using ModelException = Com.Microsoft.Sonoma.Crashes.Ingestion.Models.Exception;
    using ModelStackFrame = Com.Microsoft.Sonoma.Crashes.Ingestion.Models.StackFrame;

    class PlatformCrashes : PlatformCrashesBase
    {
        public override Type BindingType => typeof(AndroidCrashes);

        public override bool Enabled
        {
            get { return AndroidCrashes.Enabled; }
            set { AndroidCrashes.Enabled = value; }
        }

        public override bool HasCrashedInLastSession => AndroidCrashes.HasCrashedInLastSession;

        public override void TrackException(Exception exception)
        {
            AndroidCrashes.Instance.TrackException(GenerateModelException(exception));
        }

        private static readonly ModelStackFrame EmptyModelFrame = new ModelStackFrame();

        private static ManagedErrorLog _errorLog;

        private static Exception _exception;

        static PlatformCrashes()
        {
            Log.Info("SonomaXamarin", "Set up Xamarin crash handler.");
            AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledException;
            AndroidCrashes.Instance.SetWrapperSdkListener(new CrashListener());
        }

        private static void OnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            _exception = e.Exception;
            Log.Error("SonomaXamarin", "Xamarin crash " + _exception);
            JoinExceptionAndLog();
        }

        private static void JoinExceptionAndLog()
        {
            if (_errorLog != null && _exception != null)
            {
                _errorLog.Exception = GenerateModelException(_exception);
                AndroidCrashes.Instance.SaveWrapperSdkErrorLog(_errorLog);
            }
        }

        private static ModelException GenerateModelException(Exception exception)
        {
            var modelException = new ModelException
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                Frames = GenerateModelStackFrames(new StackTrace(exception, true))
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
                    ClassName = frame.GetMethod()?.DeclaringType?.ToString(),
                    MethodName = frame.GetMethod()?.Name,
                    FileName = frame.GetFileName(),
                    LineNumber = frame.GetFileLineNumber() != 0 ? new Java.Lang.Integer(frame.GetFileLineNumber()) : null
                }).Where(modelFrame => !modelFrame.Equals(EmptyModelFrame)));
            }
            return modelFrames;
        }

        private class CrashListener : Java.Lang.Object, AndroidCrashes.IWrapperSdkListener
        {
            public void OnCrashCaptured(ManagedErrorLog errorLog)
            {
                _errorLog = errorLog;
                JoinExceptionAndLog();
            }
        }
    }
}