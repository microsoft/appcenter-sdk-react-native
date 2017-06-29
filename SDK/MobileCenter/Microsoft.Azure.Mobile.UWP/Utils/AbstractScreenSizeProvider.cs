using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    internal abstract class AbstractScreenSizeProvider : IScreenSizeProvider
    {
        public abstract int Height { get; }
        public abstract int Width { get; }

        public abstract event EventHandler ScreenSizeChanged;

        public string ScreenSize
        {
            get { return Height == -1 || Width == -1 ? "unknown" : $"{Width}x{Height}"; }
        }

        public abstract Task<bool> IsAvaliableAsync(TimeSpan timeout);
    }
}
