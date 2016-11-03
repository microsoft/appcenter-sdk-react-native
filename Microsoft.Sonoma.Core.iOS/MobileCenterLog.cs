namespace Microsoft.Azure.Mobile
{
	using iOSMessageProvider = Microsoft.Azure.Mobile.iOS.Bindings.MSLogMessageProvider;
	using iOSLogger = Microsoft.Azure.Mobile.iOS.Bindings.MSWrapperLogger;

	public static partial class MobileCenterlog
	{
		public static void Verbose(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel.Verbose);
		}

		public static void Debug(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel.Debug);
		}

		public static void Info(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel.Info);
		}

		public static void Warn(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel.Warning);
		}

        public static void Error(string tag, string message)
        {
            iOSMessageProvider msg_provider = () => { return message; };
            iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel.Error);
        }

        public static void Assert(string tag, string message)
        {
            iOSMessageProvider msg_provider = () => { return message; };
            iOSLogger.MSWrapperLog(msg_provider, tag, Microsoft.Azure.Mobile.iOS.Bindings.MSLogLevel.Assert);
        }
	}
}
