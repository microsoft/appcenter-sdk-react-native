// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace Microsoft.AppCenter.Utils
{
    public class DefaultScreenSizeProvider : ScreenSizeProviderBase
    {
        // Timeout choice is arbitrary, but it is better than pausing the SDK indefinitely
        // when running without ever starting the UI.
        private readonly TimeSpan _displayInformationTimeout = TimeSpan.FromSeconds(3);
        private readonly SemaphoreSlim _displayInformationEventSemaphore = new SemaphoreSlim(0);
        private readonly object _lockObject = new object();

        // Either of these == -1 translates to screen size of null.
        private int _cachedScreenHeight = -1;
        private int _cachedScreenWidth = -1;
        private const string FailureMessage = "Could not determine display size.";

        public DefaultScreenSizeProvider()
        {
            if (!ApiInformation.IsPropertyPresent(typeof(DisplayInformation).FullName, "ScreenHeightInRawPixels") ||
                !ApiInformation.IsPropertyPresent(typeof(DisplayInformation).FullName, "ScreenWidthInRawPixels"))
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, FailureMessage);
                _displayInformationEventSemaphore.Release();
                return;
            }

            // Only try to get screen size once resuming event is invoked, because there's no point
            // in trying beforehand.
            ApplicationLifecycleHelper.Instance.ApplicationResuming += SetUpDisplayInformation;
        }

        // Subscribe to the proper events and try to 
        private void SetUpDisplayInformation(object sender, EventArgs e)
        {
            try
            {
                CoreApplication.MainView?.CoreWindow?.Dispatcher?.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            // The exceptions that display information can throw are not documented,
                            // so a catch-all is necessary.
                            var displayInfo = DisplayInformation.GetForCurrentView();
                            UpdateDisplayInformation((int)displayInfo.ScreenHeightInRawPixels,
                                (int)displayInfo.ScreenWidthInRawPixels);

                            // Try to detect a change in screen size by attaching handlers to these events.
                            // Since this code can execute multiple times on the same displayInfo, prevent
                            // duplicate handlers by removing and then setting them.
                            displayInfo.OrientationChanged -= UpdateDisplayInformationHandler;
                            displayInfo.OrientationChanged += UpdateDisplayInformationHandler;
                            displayInfo.DpiChanged -= UpdateDisplayInformationHandler;
                            displayInfo.DpiChanged += UpdateDisplayInformationHandler;
                            displayInfo.ColorProfileChanged -= UpdateDisplayInformationHandler;
                            displayInfo.ColorProfileChanged += UpdateDisplayInformationHandler;

                            // If everything succeeded, we must unsubscribe from the resuming event.
                            ApplicationLifecycleHelper.Instance.ApplicationResuming -= SetUpDisplayInformation;
                        }
                        catch
                        {
                            AppCenterLog.Warn(AppCenterLog.LogTag, FailureMessage);
                        }
                        finally
                        {
                            _displayInformationEventSemaphore.Release();
                        }
                    });
            }
            catch (COMException)
            {
                // This is reached if the MainView is not ready to be accessed yet.
                _displayInformationEventSemaphore.Release();
                AppCenterLog.Warn(AppCenterLog.LogTag, FailureMessage);
            }
        }

        private void UpdateDisplayInformationHandler(DisplayInformation displayInfo, object e)
        {
            UpdateDisplayInformation((int)displayInfo.ScreenHeightInRawPixels, (int)displayInfo.ScreenWidthInRawPixels);
        }

        internal Task UpdateDisplayInformation(int newScreenHeight, int newScreenWidth)
        {
            lock (_lockObject)
            {
                var newHeight = newScreenHeight;
                var newWidth = newScreenWidth;
                var resolutionChanged = newHeight != _cachedScreenHeight || newWidth != _cachedScreenWidth;
                _cachedScreenHeight = newHeight;
                _cachedScreenWidth = newWidth;
                if (resolutionChanged)
                {
                    // Don't want to invoke this on the UI thread, so wrap in a task to be safe.
                    return Task.Run(() =>
                    {
                        ScreenSizeChanged?.Invoke(null, EventArgs.Empty);
                    });
                }
                return Task.CompletedTask;
            }
        }

        public override int Height => _cachedScreenHeight;

        public override int Width => _cachedScreenWidth;

        public override async Task WaitUntilReadyAsync()
        {
            await _displayInformationEventSemaphore.WaitAsync(_displayInformationTimeout).ConfigureAwait(false);
            _displayInformationEventSemaphore.Release();
        }

        public override event EventHandler ScreenSizeChanged;
    }
}
