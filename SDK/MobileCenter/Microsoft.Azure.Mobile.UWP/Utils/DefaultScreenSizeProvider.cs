using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;

namespace Microsoft.Azure.Mobile.Utils
{
    class DefaultScreenSizeProvider : AbstractScreenSizeProvider
    {
        private readonly SemaphoreSlim DisplayInformationEventSemaphore = new SemaphoreSlim(0);
        private readonly TimeSpan DisplayInformationTimeout = TimeSpan.FromSeconds(2);
        private readonly object LockObject = new object();

        private int _cachedScreenHeight;
        private int _cachedScreenWidth;

        private bool _didSetUpScreenSizeEvent;
        private readonly bool CanReadScreenSize;
        //TODO unknown default
        public DefaultScreenSizeProvider()
        {
            CanReadScreenSize =
                ApiInformation.IsPropertyPresent(typeof(DisplayInformation).FullName, "ScreenHeightInRawPixels") &&
                ApiInformation.IsPropertyPresent(typeof(DisplayInformation).FullName, "ScreenWidthInRawPixels");

            // This must all be done from the leaving background event because DisplayInformation can only be used
            // from the main thread
            if (CanReadScreenSize &&
                ApiInformation.IsEventPresent(typeof(CoreApplication).FullName, "LeavingBackground"))
            {
                CoreApplication.LeavingBackground += (sender, e) =>
                {
                    lock (LockObject)
                    {
                        if (_didSetUpScreenSizeEvent)
                        {
                            return;
                        }

                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () =>
                                {
                                    DisplayInformation.GetForCurrentView().OrientationChanged +=
                                        (displayInfo, obj) =>
                                        {
                                            // Do this in a task to avoid a deadlock
                                            Task.Run(() =>
                                            {
                                                RefreshDisplayCache().Wait();
                                            });
                                        };
                                })
                            .AsTask()
                            .Wait();

                        _didSetUpScreenSizeEvent = true;

                        // Do this in a task to avoid a deadlock
                        Task.Run(() =>
                        {
                            RefreshDisplayCache().Wait();
                            DisplayInformationEventSemaphore.Release();
                        });
                    }
                };
            }
        }

        public async Task RefreshDisplayCache()
        {
            // This can only succeed on the UI thread due to limitations of
            // the DisplayInformation class
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    lock (LockObject)
                    {
                        DisplayInformation displayInfo = null;
                        try
                        {
                            // This can throw exceptions that aren't well documented, so catch-all and ignore
                            displayInfo = DisplayInformation.GetForCurrentView();
                        }
                        catch (Exception e)
                        {
                            MobileCenterLog.Warn(MobileCenterLog.LogTag, "Could not get display information.", e);
                            return;
                        }
                        if (_cachedScreenHeight == (int)displayInfo.ScreenHeightInRawPixels &&
                            _cachedScreenWidth == (int)displayInfo.ScreenWidthInRawPixels)
                        {
                            return;
                        }
                        _cachedScreenHeight = (int)displayInfo.ScreenHeightInRawPixels;
                        _cachedScreenWidth = (int)displayInfo.ScreenWidthInRawPixels;

                        ScreenSizeChanged?.Invoke(null, EventArgs.Empty);
                    }
                });
        }

        public override int Height
        {
            get { return _cachedScreenHeight; }
        }

        public override int Width
        {
            get { return _cachedScreenWidth; }
        }

        public override async Task<bool> IsAvaliableAsync(TimeSpan timeout)
        {
            return await DisplayInformationEventSemaphore.WaitAsync(DisplayInformationTimeout).ConfigureAwait(false);
        }

        public override event EventHandler ScreenSizeChanged;
    }
}
