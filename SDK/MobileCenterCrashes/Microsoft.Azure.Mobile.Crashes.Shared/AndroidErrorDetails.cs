namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Error report details pertinent only to devices running Android.
    /// </summary>
    public class AndroidErrorDetails
    {
        internal AndroidErrorDetails(object throwable, string threadName)
        {
            Throwable = throwable;
            ThreadName = threadName;
        }

        /// <summary>
        /// Gets the throwable associated with the Java crash.
        /// </summary>
        /// <value>The throwable associated with the crash. <c>null</c> if the crash occured in .NET code.</value>
        public object Throwable { get; }

        /// <summary>
        /// Gets the name of the thread that crashed.
        /// </summary>
        /// <value>The name of the thread that crashed.</value>
        public string ThreadName { get; }
    }
}
