using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Exception type thrown for testing purposes. See <see cref="Crashes.GenerateTestCrash"/>. 
    /// </summary>
    public class TestCrashException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TestCrashException class with a predefined error message.
        /// </summary>
        public TestCrashException()
        {
            throw new NotImplementedException();
        }
    }
}
