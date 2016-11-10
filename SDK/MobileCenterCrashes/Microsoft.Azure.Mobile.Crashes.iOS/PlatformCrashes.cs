using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;
using Foundation;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Mobile.Crashes
{
    class PlatformCrashes : PlatformCrashesBase
    {
        public override SendingErrorReportHandler SendingErrorReport { get; set; }
        public override SentErrorReportHandler SentErrorReport { get; set; }
        public override FailedToSendErrorHandler FailedToSendErrorReport { get; set; }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        public override GetErrorAttachmentCallback GetErrorAttachment { get; set; }

        CrashesDelegate crashesDelegate { get; set; }

        public override Type BindingType => typeof(MSCrashes);

        public override bool Enabled
        {
            get { return MSCrashes.IsEnabled(); }
            set { MSCrashes.SetEnabled(value); }
        }

        public override bool HasCrashedInLastSession => MSCrashes.HasCrashedInLastSession;

        public override ErrorReport LastSessionCrashReport
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //public override void TrackException(Exception exception)
        //{
        //	throw new NotImplementedException();
        //}

        static PlatformCrashes()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        public PlatformCrashes()
        {
            crashesDelegate = new CrashesDelegate(this);
            MSCrashes.SetDelegate(crashesDelegate);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MSException exception = GenerateiOSException((Exception)e.ExceptionObject);
            MSWrapperExceptionManager.SetWrapperException(exception);
        }

        private static MSException GenerateiOSException(Exception exception)
        {
            var msException = new MSException();
            msException.Type = exception.GetType().FullName;
            msException.Message = exception.Message;
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
