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
        event EventHandler<object> ApplicationSuspending;

        /// <summary>
        /// Occurs when the application is about to resume
        /// </summary>
        event EventHandler<object> ApplicationResuming;

        /// <summary>
        /// Occurs when an unhandled exception is fired
        /// </summary>
        event UnhandledExceptionOccurredEventHandler UnhandledExceptionOccurred;
    }
}
