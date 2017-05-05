using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Push.Windows.Shared
{
    /// <summary>
    /// Contains event data associated with a push notification event
    /// </summary>
    public class PushNotificationEventArgs 
    {
        /// <summary>
        /// Notification title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Notification message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Custom data
        /// </summary>
        public IDictionary<string, string> CustomData { get; set; }
    }
}
