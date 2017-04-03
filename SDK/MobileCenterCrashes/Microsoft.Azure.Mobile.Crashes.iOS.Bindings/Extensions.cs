namespace Microsoft.Azure.Mobile.Crashes.iOS.Bindings
{
    /*
     * This class is required so that Mono can handle the signals SIGSEGV and SIGBUS, which should not always
     * cause a crash, but do if the native SDK's crash reporting service handles them.
     */
    public class CrashesInitializationDelegate : MSWrapperCrashesInitializationDelegate
    {
        [DllImport("libc")]
        private static extern int sigaction(Signal sig, IntPtr act, IntPtr oact);

        private enum Signal
        {
            SIGFPE = 8,
            SIGBUS = 10,
            SIGSEGV = 11
        }

        /* This constructor is required for Mono's internal purposes. Deleting it can cause crashes. */
        public CrashesInitializationDelegate(IntPtr handle) : base(handle)
        {
        }

        public CrashesInitializationDelegate()
        {
        }

        public override bool SetUpCrashHandlers()
        {
            if (!TryChainingSignalHandlers())
            {
                OverwriteSignalHandlers();
            }
            return true;
        }

        /* 
         * In Mono 4.8, it is possible to chain the mono signal handlers to the PLCrashReporter signal handlers, so
         * if the APIs for this are available, it is preferable to use them. If the APIs are unavailable, return
         * false.
         */
        private bool TryChainingSignalHandlers()
        {
            var type = Type.GetType("Mono.Runtime");
            var installSignalHandlers = type?.GetMethod("InstallSignalHandlers", BindingFlags.Public | BindingFlags.Static);
            var removeSignalHandlers = type?.GetMethod("RemoveSignalHandlers", BindingFlags.Public | BindingFlags.Static);

            if (installSignalHandlers == null || removeSignalHandlers == null)
            {
                return false;
            }

            try
            {
            }
            finally
            {
                removeSignalHandlers.Invoke(null, null);
                try
                {
                    MSWrapperExceptionManager.StartCrashReportingFromWrapperSdk();
                }
                finally
                {
                    installSignalHandlers.Invoke(null, null);
                }
            }

            return true;
        }

        /*
         * If the Mono 4.8 APIs are unavailable, we must overwrite the signal handlers for certain signals. 
         */
        private void OverwriteSignalHandlers()
        {
            /* Allocate space to store the Mono handlers */
            var sigbus = Marshal.AllocHGlobal(512);
            var sigsegv = Marshal.AllocHGlobal(512);
            var sigfpe = Marshal.AllocHGlobal(512);

            /* Store Mono's SIGSEGV, SIGBUS, and SIGFPE handlers */
            sigaction(Signal.SIGBUS, IntPtr.Zero, sigbus);
            sigaction(Signal.SIGSEGV, IntPtr.Zero, sigsegv);
            sigaction(Signal.SIGFPE, IntPtr.Zero, sigfpe);

            /* Enable native SDK crash reporting library */
            MSWrapperExceptionManager.StartCrashReportingFromWrapperSdk();

            /* Restore Mono SIGSEGV, SIGBUS, and SIGFPE handlers */
            sigaction(Signal.SIGBUS, sigbus, IntPtr.Zero);
            sigaction(Signal.SIGSEGV, sigsegv, IntPtr.Zero);
            sigaction(Signal.SIGFPE, sigfpe, IntPtr.Zero);

            /* Release previously allocated space */
            Marshal.FreeHGlobal(sigbus);
            Marshal.FreeHGlobal(sigsegv);
            Marshal.FreeHGlobal(sigfpe);
        }
    }
}
