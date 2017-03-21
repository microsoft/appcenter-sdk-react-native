using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public interface IApplicationLifecycleHelper
    {
        /// <summary>
        /// Gets or sets whether the Application lifecycle events fire (but unhandled exception event always fires)
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Occurs when the application is about to be suspended
        /// </summary>
        event EventHandler ApplicationSuspending;

        /// <summary>
        /// Occurs when the application is about to resume
        /// </summary>
        event EventHandler ApplicationResuming;

        /// <summary>
        /// Occurs when an unhandled exception is fired
        /// </summary>
        event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
