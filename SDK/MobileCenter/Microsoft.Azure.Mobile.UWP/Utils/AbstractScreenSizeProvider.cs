using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    abstract class AbstractScreenSizeProvider : IScreenSizeProvider
    {
        public abstract int Height { get; }
        public abstract int Width { get; }

        public abstract event EventHandler ScreenSizeChanged;

        public string ScreenSize
        {
            get { return $"{Width}x{Height}"; }
        }

        public abstract Task<bool> IsAvaliableAsync(TimeSpan timeout);
    }
}
