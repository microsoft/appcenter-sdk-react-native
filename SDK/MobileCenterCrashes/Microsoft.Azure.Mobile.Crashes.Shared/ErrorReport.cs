namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Error report containing information about a particular crash.
    /// </summary>
    public partial class ErrorReport
    {
        /// <summary>
        /// Gets the report identifier.
        /// </summary>
        /// <value>UUID for the report.</value>
        public string Id { get; }

        /// <summary>
        /// Gets the app start time.
        /// </summary>
        /// <value>Date and time the app started</value>
        public DateTimeOffset AppStartTime { get; }

        /// <summary>
        /// Gets the app error time.
        /// </summary>
        /// <value>Date and time the error occured</value>
        public DateTimeOffset AppErrorTime { get; }

        /// <summary>
        /// Gets the device that the crashed app was being run on.
        /// </summary>
        /// <value>Device information at the crash time.</value>
        public Device Device { get; }

        /// <summary>
        /// Gets the C# Exception object that caused the crashed.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; }

        /// <summary>
        /// Gets details specific to Android.
        /// </summary>
        /// <value>Android error report details. <c>null</c> if the OS is not Android.</value>
        public AndroidErrorDetails AndroidDetails { get; }

        /// <summary>
        /// Gets details specific to iOS.
        /// </summary>
        /// <value>iOS error report details. <c>null</c> if the OS is not iOS.</value>
        public iOSErrorDetails iOSDetails { get; }
    }
}
