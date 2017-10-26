﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Crashes.iOS.Bindings
{
    /*
     * This class is required so that Mono can handle the signals SIGSEGV and SIGBUS, which should not always
     * cause a crash, but do if the native SDK's crash reporting service handles them.
     */
    public class CrashesInitializationDelegate : MSCrashHandlerSetupDelegate
    {
        [DllImport("libc")]
        private static extern int sigaction(Signal sig, IntPtr act, IntPtr oact);

        private enum Signal
        {
            SIGFPE = 8,
            SIGBUS = 10,
            SIGSEGV = 11
        }

        IntPtr sigbus;
        IntPtr sigsegv;
        IntPtr sigfpe;

        /* This constructor is required for Mono's internal purposes. Deleting it can cause crashes. */
        public CrashesInitializationDelegate(IntPtr handle) : base(handle)
        {
        }

        public CrashesInitializationDelegate()
        {
        }

        // In Mono 4.8, it is possible to chain the mono signal handlers to the PLCrashReporter signal handlers, so
        // if the APIs for this are available, it is preferable to use them.
        public override void WillSetUpCrashHandlers()
        {
            var type = Type.GetType("Mono.Runtime");
            var removeSignalHandlers = type?.GetMethod("RemoveSignalHandlers", BindingFlags.Public | BindingFlags.Static);
            if (removeSignalHandlers != null)
            {
                removeSignalHandlers.Invoke(null, null);
                return;
            }

            // Mono 4.8+ APIs must be unavailable

            // Allocate space to store the Mono handlers
            sigbus = Marshal.AllocHGlobal(512);
            sigsegv = Marshal.AllocHGlobal(512);
            sigfpe = Marshal.AllocHGlobal(512);

            // Store Mono's SIGSEGV, SIGBUS, and SIGFPE handlers
            sigaction(Signal.SIGBUS, IntPtr.Zero, sigbus);
            sigaction(Signal.SIGSEGV, IntPtr.Zero, sigsegv);
            sigaction(Signal.SIGFPE, IntPtr.Zero, sigfpe);
        }

        // In Mono 4.8, it is possible to chain the mono signal handlers to the PLCrashReporter signal handlers, so
        // if the APIs for this are available, it is preferable to use them.
        public override void DidSetUpCrashHandlers()
        {
            var type = Type.GetType("Mono.Runtime");
            var installSignalHandlers = type?.GetMethod("InstallSignalHandlers", BindingFlags.Public | BindingFlags.Static);
            if (installSignalHandlers != null)
            {
                installSignalHandlers.Invoke(null, null);
                return;
            }

            // Mono 4.8+ APIs must be unavailable

            // Restore Mono SIGSEGV, SIGBUS, and SIGFPE handlers
            sigaction(Signal.SIGBUS, sigbus, IntPtr.Zero);
            sigaction(Signal.SIGSEGV, sigsegv, IntPtr.Zero);
            sigaction(Signal.SIGFPE, sigfpe, IntPtr.Zero);

            // Release previously allocated space
            Marshal.FreeHGlobal(sigbus);
            Marshal.FreeHGlobal(sigsegv);
            Marshal.FreeHGlobal(sigfpe);
        }

        public override bool ShouldEnableUncaughtExceptionHandler()
        {
            return false;
        }
    }
}
