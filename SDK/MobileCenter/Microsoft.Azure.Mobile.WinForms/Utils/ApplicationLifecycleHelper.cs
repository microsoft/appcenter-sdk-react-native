using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
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
        private static readonly dynamic WPFApplication;
        private static readonly int WPFMinimizedState;
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly presentationFramework = null;
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name == "PresentationFramework")
                {
                    presentationFramework = assembly;
                    break;
                }
            }
            if (presentationFramework != null)
            {
                var appType = presentationFramework.GetType("System.Windows.Application");
                WPFApplication = appType.GetRuntimeProperty("Current")?.GetValue(appType);

                WPFMinimizedState = (int)presentationFramework.GetType("System.Windows.WindowState")
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
            if (WPFApplication != null)
            {
                foreach (dynamic window in WPFApplication.Windows)
                {
                    if ((int)window.WindowState != WPFMinimizedState && WindowIntersectsWithAnyScreen(window))
                    {
                        return true;
                    }
                }
                return false;
            }

            // Running on WinForms
            return Application.OpenForms.Cast<Form>().Any(form => form.WindowState != FormWindowState.Minimized);

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
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.IntersectsWith(windowBounds))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasShownWindow => started;

        public bool IsSuspended => suspended;

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler ApplicationStarted;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
