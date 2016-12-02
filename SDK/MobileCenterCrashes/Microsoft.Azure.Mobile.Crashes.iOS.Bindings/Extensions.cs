using System;
using System.Runtime.InteropServices;

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
            SIGBUS = 10,
            SIGSEGV = 11
        }

        public override bool SetUpCrashHandlers()
        {
            /* Allocate space to store the Mono handlers */
            IntPtr sigbus = Marshal.AllocHGlobal(512);
            IntPtr sigsegv = Marshal.AllocHGlobal(512);

            /* Store Mono's SIGSEGV and SIGBUS handlers */
            sigaction(Signal.SIGBUS, IntPtr.Zero, sigbus);
            sigaction(Signal.SIGSEGV, IntPtr.Zero, sigsegv);

            /* Enable native SDK crash reporting library */
            MSWrapperExceptionManager.StartCrashReportingFromWrapperSdk();

            /* Restore Mono SIGSEGV and SIGBUS handlers */
            sigaction(Signal.SIGBUS, sigbus, IntPtr.Zero);
            sigaction(Signal.SIGSEGV, sigsegv, IntPtr.Zero);
            Marshal.FreeHGlobal(sigbus);
            Marshal.FreeHGlobal(sigsegv);
            return true;
        }
    }
}
