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
        private readonly SemaphoreSlim _displayInformationEventSemaphore = new SemaphoreSlim(0);
        private readonly object _lockObject = new object();

        private int _cachedScreenHeight;
        private int _cachedScreenWidth;

        private bool _didSetUpScreenSizeEvent;
        private readonly bool CanReadScreenSize;
        //TODO unknown default

        public DefaultScreenSizeProvider()
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
                {
                    var displayInfo = DisplayInformation.GetForCurrentView();
                    _cachedScreenHeight = (int) displayInfo.ScreenHeightInRawPixels;
                    _cachedScreenWidth = (int) displayInfo.ScreenWidthInRawPixels;
                    System.Diagnostics.Debug.WriteLine("Screen size is now " + ScreenSize);
                    _displayInformationEventSemaphore.Release();
                }, new CancellationToken(), TaskCreationOptions.PreferFairness, context)
                .ContinueWith(
                    (task) => ScreenSizeChanged?.Invoke(null, EventArgs.Empty));
        }

        /*
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
                    lock (_lockObject)
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
                            _displayInformationEventSemaphore.Release();
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
                    lock (_lockObject)
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
        */
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
            return await _displayInformationEventSemaphore.WaitAsync(timeout).ConfigureAwait(false);
        }

        public override event EventHandler ScreenSizeChanged;
    }
}
