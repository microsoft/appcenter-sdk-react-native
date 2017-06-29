using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    public abstract class AbstractScreenSizeProvider : IScreenSizeProvider
    {
        // Display height in pixels; -1 indicates unknown
        public abstract int Height { get; }

        // Display width in pixels; -1 indicates unkown
        public abstract int Width { get; }

        // Invoked when screen resolution changes (best-effort)
        public abstract event EventHandler ScreenSizeChanged;

        // Screen size
        public string ScreenSize
        {
            get { return Height == -1 || Width == -1 ? "unknown" : $"{Width}x{Height}"; }
        }

        // Waits until the screen size is available (until timeout)
        public abstract Task<bool> IsAvaliableAsync(TimeSpan timeout);
    }
}
