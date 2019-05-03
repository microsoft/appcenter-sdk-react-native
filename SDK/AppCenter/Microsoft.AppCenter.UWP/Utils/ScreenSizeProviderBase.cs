// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Utils
{
    public abstract class ScreenSizeProviderBase : IScreenSizeProvider
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
            get { return Height == -1 || Width == -1 ? null : $"{Width}x{Height}"; }
        }

        // Waits until the screen size is available (or definitely unavailable)
        public abstract Task WaitUntilReadyAsync();
    }
}
