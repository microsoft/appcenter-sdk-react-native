using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    /// <summary>
    /// In most UWP apps, the DefaultScreenSizeProvider will do, but some
    /// applications need to use different techniques to get the screen
    /// size (e.g., Unity)
    /// </summary>
    public interface IScreenSizeProvider
    {
        // Format must be "{Width}x{Height}"
        string ScreenSize { get; }

        // Wait until screen size is available (up to a timeout)
        Task<bool> IsAvaliableAsync(TimeSpan timeout);

        // Indicates the screen resolution has changed
        event EventHandler ScreenSizeChanged;
    }
}
