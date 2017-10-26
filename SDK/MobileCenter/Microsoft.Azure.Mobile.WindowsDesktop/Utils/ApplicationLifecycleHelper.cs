using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        // Singleton instance of ApplicationLifecycleHelper
        private static ApplicationLifecycleHelper _instance;
        public static ApplicationLifecycleHelper Instance
        {
            get { return _instance ?? (_instance = new ApplicationLifecycleHelper()); }

            // Setter for testing
            internal set { _instance = value; }
        }

        #region WinEventHook

        private delegate void WinEventDelegate(IntPtr winEventHookHandle, uint eventType, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr eventHookAssemblyHandle, WinEventDelegate eventHookHandle, uint processId, uint threadId, uint dwFlags);
        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        private static WinEventDelegate hookDelegate = new WinEventDelegate(WinEventHook);
        private static bool suspended = false;
        private static bool started = false;
        private static Action Minimize;
        private static Action Restore;
        private static Action Start;
        private static readonly dynamic WpfApplication;
        private static readonly int WpfMinimizedState;
        private static void WinEventHook(IntPtr winEventHookHandle, uint eventType, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
        {
            // Filter out non-HWND
            if (objectId != 0 || childId != 0)
            {
                return;
            }

            var anyNotMinimized = IsAnyWindowNotMinimized();

            if (!started && anyNotMinimized)
            {
                started = true;
                Start?.Invoke();
            }
            if (suspended && anyNotMinimized)
            {
                suspended = false;
                Restore?.Invoke();
            }
            else if (!suspended && !anyNotMinimized)
            {
                suspended = true;
                Minimize?.Invoke();
            }
        }

        static ApplicationLifecycleHelper()
        {
            // Retrieve the WPF APIs through reflection, if they are available
            if (WpfHelper.IsRunningOnWpf)
            {
                // Store the WPF Application singleton
                // This is equivalent to `WpfApplication = System.Windows.Application.Current;`
                var appType = WpfHelper.PresentationFramework.GetType("System.Windows.Application");
                WpfApplication = appType.GetRuntimeProperty("Current")?.GetValue(appType);

                // Store the int corresponding to the "Minimized" state for WPF Windows
                // This is equivalent to `WpfMinimizedState = (int)System.Windows.WindowState.Minimized;`
                WpfMinimizedState = (int)WpfHelper.PresentationFramework.GetType("System.Windows.WindowState")
                    .GetField("Minimized")
                    .GetRawConstantValue();
            }

            
#pragma warning disable CS0618 // Type or member is obsolete
            // We need Windows thread ID, not managed
            var threadId = AppDomain.GetCurrentThreadId();
#pragma warning restore CS0618 // Type or member is obsolete
            var hook = SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, hookDelegate, 0, (uint)threadId, WINEVENT_OUTOFCONTEXT);
            Application.ApplicationExit += delegate { UnhookWinEvent(hook); };
        }

        private static bool IsAnyWindowNotMinimized()
        {
            // If not in WPF, query the available forms
            if (WpfApplication == null)
            {
                return Application.OpenForms.Cast<Form>().Any(form => form.WindowState != FormWindowState.Minimized);
            }

            // If in WPF, query the available windows
            foreach (var window in WpfApplication.Windows)
            {
                // Not minimized is true if WindowState is not "Minimized" and the window is on screen
                if ((int)window.WindowState != WpfMinimizedState && WindowIntersectsWithAnyScreen(window))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public ApplicationLifecycleHelper()
        {
            Enabled = true;
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs((Exception)eventArgs.ExceptionObject));
            };
        }

        private void InvokeResuming()
        {
            ApplicationResuming?.Invoke(null, EventArgs.Empty);
        }

        private void InvokeStarted()
        {
            ApplicationStarted?.Invoke(null, EventArgs.Empty);
        }

        private void InvokeSuspended()
        {
            ApplicationSuspended?.Invoke(null, EventArgs.Empty);
        }

        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (value == enabled)
                {
                    return;
                }
                if (value)
                {
                    Start = InvokeStarted;
                    Restore = InvokeResuming;
                    Minimize = InvokeSuspended;
                }
                else
                {
                    Start = null;
                    Restore = null;
                    Minimize = null;
                }
                enabled = value;
            }
        }

        private static Rectangle WindowsRectToRectangle(dynamic windowsRect)
        {
            return new Rectangle
            {
                X = (int)windowsRect.X,
                Y = (int)windowsRect.Y,
                Width = (int)windowsRect.Width,
                Height = (int)windowsRect.Height
            };
        }

        private static bool WindowIntersectsWithAnyScreen(dynamic window)
        {
            var windowBounds = WindowsRectToRectangle(window.RestoreBounds);
            return Screen.AllScreens.Any(screen => screen.Bounds.IntersectsWith(windowBounds));
        }

        public bool HasShownWindow => started;

        public bool IsSuspended => suspended;

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler ApplicationStarted;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
