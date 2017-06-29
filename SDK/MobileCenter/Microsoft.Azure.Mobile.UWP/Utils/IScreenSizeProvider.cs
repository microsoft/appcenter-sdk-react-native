using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    public interface IScreenSizeProvider
    {
        string ScreenSize { get; }
        Task<bool> IsAvaliableAsync(TimeSpan timeout);

        event EventHandler ScreenSizeChanged;
    }
}
