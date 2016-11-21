using System;
using System.Runtime.InteropServices;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{   
    public static partial class Crashes
    {
        public static void ApplyDelegate()
        {
            MSWrapperExceptionManager.SetDelegate(new InitializationDelegate());
        }

        /*
         * This is required so that Mono can handle the signals SIGSEGV and SIGBUS, which should not always
         * cause a crash, but do if the native SDK's crash reporting service handles them.
         */
        public class InitializationDelegate : MSWrapperCrashesInitializationDelegate
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
                IntPtr sigbus = Marshal.AllocHGlobal(512);
                IntPtr sigsegv = Marshal.AllocHGlobal(512);

                // Store Mono's SIGSEGV and SIGBUS handlers
                sigaction(Signal.SIGBUS, IntPtr.Zero, sigbus);
                sigaction(Signal.SIGSEGV, IntPtr.Zero, sigsegv);

                // Enable native SDK crash reporting library
                MSWrapperExceptionManager.StartCrashReportingFromWrapperSdk();

                // Restore Mono SIGSEGV and SIGBUS handlers
                sigaction(Signal.SIGBUS, sigbus, IntPtr.Zero);
                sigaction(Signal.SIGSEGV, sigsegv, IntPtr.Zero);

                Marshal.FreeHGlobal(sigbus);
                Marshal.FreeHGlobal(sigsegv);
                return true;
            }
        }
    }
}
