using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{
    class PlatformCrashes : PlatformCrashesBase
    {
        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }
        public override SentErrorReportEventHandler SentErrorReport { get; set; }
        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        public override GetErrorAttachmentsCallback GetErrorAttachments { get; set; }
        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

        CrashesDelegate crashesDelegate { get; set; }

        public override Type BindingType => typeof(MSCrashes);

        public override Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(MSCrashes.IsEnabled());
        }

        public override Task SetEnabledAsync(bool enabled)
        {
            MSCrashes.SetEnabled(enabled);
            return Task.FromResult((object)null);
        }

        public override Task<bool> HasCrashedInLastSessionAsync()
        {
            return Task.FromResult(MSCrashes.HasCrashedInLastSession);
        }

        public override Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return Task.Run(() =>
            {
                var msReport = MSCrashes.LastSessionCrashReport;
                if (msReport == null)
                    return null;
                return ErrorReportCache.GetErrorReport(msReport);
            });
        }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
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

        //public override void TrackException(Exception exception)
        //{
        //	throw new NotImplementedException();
        //}

        static PlatformCrashes()
        {
            /* Peform custom setup around the native SDK's for setting signal handlers */
            MSWrapperExceptionManager.SetDelegate(new CrashesInitializationDelegate());
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        public PlatformCrashes()
        {
            MSCrashes.SetUserConfirmationHandler((arg0) =>
                    {
                        if (ShouldAwaitUserConfirmation != null)
                        {
                            return ShouldAwaitUserConfirmation();
                        }
                        return false;
                    });
            crashesDelegate = new CrashesDelegate(this);
            MSCrashes.SetDelegate(crashesDelegate);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception systemException = e.ExceptionObject as Exception;
            MSException exception = GenerateiOSException(systemException);
            MSWrapperExceptionManager.SetWrapperException(exception);

            byte[] exceptionBytes = CrashesUtils.SerializeException(systemException);
            NSData wrapperExceptionData = NSData.FromArray(exceptionBytes);
            MSWrapperExceptionManager.SetWrapperExceptionData(wrapperExceptionData);
        }

        private static MSException GenerateiOSException(Exception exception)
        {
            var msException = new MSException();
            msException.Type = exception.GetType().FullName;
            msException.Message = exception.Message;
            msException.StackTrace = exception.StackTrace;
            msException.Frames = GenerateStackFrames(exception);
            msException.WrapperSdkName = WrapperSdk.Name;

            var aggregateException = exception as AggregateException;
            var innerExceptions = new List<MSException>();

            if (aggregateException?.InnerExceptions != null)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    innerExceptions.Add(GenerateiOSException(innerException));
                }
            }
            else if (exception.InnerException != null)
            {
                innerExceptions.Add(GenerateiOSException(exception.InnerException));
            }

            msException.InnerExceptions = innerExceptions.Count > 0 ? innerExceptions.ToArray() : null;

            return msException;
        }

#pragma warning disable XS0001 // Find usages of mono todo items

        private static MSStackFrame[] GenerateStackFrames(Exception e)
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
            return frameList.Count == 0 ? null : frameList.ToArray();
        }

#pragma warning restore XS0001 // Find usages of mono todo items

        private static string AnonymizePath(string path)
        {
            if ((path == null) || (path.Count() == 0) || !path.Contains("/Users/"))
            {
                return path;
            }

            string pattern = "(/Users/[^/]+/)";
            return Regex.Replace(path, pattern, "/Users/USER/");
        }
    }
}
