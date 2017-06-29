using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;

namespace Microsoft.Azure.Mobile.Utils
{
    class DefaultScreenSizeProvider : ScreenSizeProviderBase
    {
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
                MobileCenterLog.Warn(MobileCenterLog.LogTag, FailureMessage);
                _displayInformationEventSemaphore.Release();
                return;
            }
            try
            {
                // CurrentSynchronization context is essentially the UI context, which is needed
                // to get display information. It isn't guaranteed to be available, so try/catch.
                var context = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Factory.StartNew(() =>
                {
                    var displayInfo = DisplayInformation.GetForCurrentView();
                    UpdateDisplayInformation(displayInfo, null);

                    // Try to detect a change in screen size by attaching handlers to these events.
                    displayInfo.OrientationChanged += UpdateDisplayInformation;
                    displayInfo.DpiChanged += UpdateDisplayInformation;
                    displayInfo.ColorProfileChanged += UpdateDisplayInformation;
                }, new CancellationToken(), TaskCreationOptions.PreferFairness, context);
            }
            catch (InvalidOperationException)
            {
                _displayInformationEventSemaphore.Release();
                MobileCenterLog.Warn(MobileCenterLog.LogTag, FailureMessage);
            }
        }
       
        private void UpdateDisplayInformation(DisplayInformation displayInfo, object e)
        {
            lock (_lockObject)
            {
                var newHeight = (int)displayInfo.ScreenHeightInRawPixels;
                var newWidth = (int)displayInfo.ScreenWidthInRawPixels;
                var resolutionChanged = newHeight != _cachedScreenHeight || newWidth != _cachedScreenWidth;
                _cachedScreenHeight = newHeight;
                _cachedScreenWidth = newWidth;
                _displayInformationEventSemaphore.Release();
                if (resolutionChanged)
                {
                    // Don't want to invoke this on the UI thread, so wrap in a task to be safe.
                    Task.Run(() =>
                    {
                        ScreenSizeChanged?.Invoke(null, EventArgs.Empty);
                    });
                }
            }
        }

        public override int Height
        {
            get { return _cachedScreenHeight; }
        }

        public override int Width
        {
            get { return _cachedScreenWidth; }
        }

        public override async Task<bool> IsAvailableAsync(TimeSpan timeout)
        {
            return await _displayInformationEventSemaphore.WaitAsync(timeout).ConfigureAwait(false);
        }

        public override event EventHandler ScreenSizeChanged;
    }
}
