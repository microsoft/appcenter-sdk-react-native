namespace Microsoft.Sonoma.Core
{
	using iOSMessageProvider = Microsoft.Sonoma.Core.iOS.Bindings.SNMLogMessageProvider;
	using iOSLogger = Microsoft.Sonoma.Core.iOS.Bindings.SNMWrapperLogger;

	public static partial class SonomaLog
	{
		public static void Verbose(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.SNMWrapperLog(msg_provider, tag, Microsoft.Sonoma.Core.iOS.Bindings.SNMLogLevel.Verbose);
		}

		public static void Debug(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.SNMWrapperLog(msg_provider, tag, Microsoft.Sonoma.Core.iOS.Bindings.SNMLogLevel.Debug);
		}


		public static void Error(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.SNMWrapperLog(msg_provider, tag, Microsoft.Sonoma.Core.iOS.Bindings.SNMLogLevel.Error);
		}

		public static void Info(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.SNMWrapperLog(msg_provider, tag, Microsoft.Sonoma.Core.iOS.Bindings.SNMLogLevel.Info);
		}

		public static void Warn(string tag, string message)
		{
			iOSMessageProvider msg_provider = () => { return message; };
			iOSLogger.SNMWrapperLog(msg_provider, tag, Microsoft.Sonoma.Core.iOS.Bindings.SNMLogLevel.Warning);
		}
	}
}
