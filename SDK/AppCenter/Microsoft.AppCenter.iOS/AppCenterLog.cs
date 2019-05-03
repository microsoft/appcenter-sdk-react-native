// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
	using iOSMessageProvider = Microsoft.AppCenter.iOS.Bindings.MSLogMessageProvider;
	using iOSLogger = Microsoft.AppCenter.iOS.Bindings.MSWrapperLogger;

	public static partial class AppCenterLog
	{
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }

        static AppCenterLog()
        {
            LogTag = "AppCenterXamarin";
        }

        public static void Verbose(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.iOS.Bindings.MSLogLevel.Verbose);
		}

		public static void Debug(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.iOS.Bindings.MSLogLevel.Debug);
		}

		public static void Info(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.iOS.Bindings.MSLogLevel.Info);
		}

		public static void Warn(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.iOS.Bindings.MSLogLevel.Warning);
		}

        public static void Error(string tag, string message)
        {
            iOSMessageProvider msg_provider = () => { return message; };
            iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.iOS.Bindings.MSLogLevel.Error);
        }

        public static void Assert(string tag, string message)
        {
            iOSMessageProvider msg_provider = () => { return message; };
            iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.iOS.Bindings.MSLogLevel.Assert);
        }
	}
}
