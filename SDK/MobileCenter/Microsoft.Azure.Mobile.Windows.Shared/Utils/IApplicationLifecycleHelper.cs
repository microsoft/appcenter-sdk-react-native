using System;

namespace Microsoft.Azure.Mobile.Utils
{
    /// <summary>
    /// Represents an object that tracks the application lifecycle.
    /// </summary>
    public interface IApplicationLifecycleHelper
    {
        /// <summary>
        /// Gets or sets whether the Application lifecycle events fire (but unhandled exception event always fires)
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Occurs when the application has just been suspended
        /// </summary>
        event EventHandler ApplicationSuspended;

        /// <summary>
        /// Occurs when the application is about to resume
        /// </summary>
        event EventHandler ApplicationResuming;

        /// <summary>
        /// Occurs when an unhandled exception is fired
        /// </summary>
        /// <remarks>This is used to set up the shutdown logic in the event of a crash.</remarks>
        event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
