using System;
using System.Threading;

namespace Microsoft.Azure.Mobile
{
    //TODO incomplete.
    public static partial class MobileCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }

        private static SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private static LogLevel _level;
        internal static LogLevel Level
        {
            get
            {
                return _level;
            }
            set
            {
                _mutex.Wait();
                _level = value;
                _mutex.Release();
            }
        }

        static MobileCenterLog()
        {
            LogTag = "MobileCenter";
        }

        public static void Verbose(string tag, string message)
        {
            _mutex.Wait();
            if (Level <= LogLevel.Verbose)
            {
                Console.WriteLine($"[{tag}] VERBOSE: {message}");
                System.Diagnostics.Debug.WriteLine($"[{tag}] VERBOSE: {message}");
            }
            _mutex.Release();
        }

        public static void Debug(string tag, string message)
        {
            _mutex.Wait();
            if (Level <= LogLevel.Debug)
            {
                Console.WriteLine($"[{tag}] DEBUG: {message}");
                System.Diagnostics.Debug.WriteLine($"[{tag}] DEBUG: {message}"); ;

            }
            _mutex.Release();
        }

        public static void Info(string tag, string message)
        {
            _mutex.Wait();
            if (Level <= LogLevel.Info)
            {
                Console.WriteLine($"[{tag}] INFO: {message}");
                System.Diagnostics.Debug.WriteLine($"[{tag}] INFO: {message}"); ;

            }
            _mutex.Release();
        }

        public static void Warn(string tag, string message)
        {
            _mutex.Wait();
            if (Level <= LogLevel.Warn)
            {
                Console.WriteLine($"[{tag}] WARN: {message}");
                System.Diagnostics.Debug.WriteLine($"[{tag}] WARN: {message}"); ;

            }
            _mutex.Release();
        }

        public static void Error(string tag, string message)
        {
            _mutex.Wait();
            if (Level <= LogLevel.Error)
            {
                Console.WriteLine($"[{tag}] ERROR: {message}");
                System.Diagnostics.Debug.WriteLine($"[{tag}] ERROR: {message}"); ;

            }
            _mutex.Release();
        }

        public static void Assert(string tag, string message)
        {
            _mutex.Wait();
            if (Level <= LogLevel.Assert)
            {
                Console.WriteLine($"[{tag}] ASSERT: {message}");
                System.Diagnostics.Debug.WriteLine($"[{tag}] ASSERT: {message}"); ;

            }
            _mutex.Release();
        }
    }
}
