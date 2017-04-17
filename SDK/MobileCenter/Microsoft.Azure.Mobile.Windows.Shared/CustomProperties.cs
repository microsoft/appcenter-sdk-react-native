using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// Custom properties builder.
    /// Collects multiple properties to send in one log.
    /// </summary>
    public class CustomProperties
    {
        private static readonly Regex KeyPattern = new Regex("^[a-zA-Z][a-zA-Z0-9]*$");

        internal Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, string value) => SetObject(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, DateTime value) => SetObject(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, int value) => SetObject(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, float value) => SetObject(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, bool value) => SetObject(key, value);

        /// <summary>
        /// Clear the property for the specified key.
        /// </summary>
        /// <param name="key">Key whose mapping is to be cleared.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Clear(string key)
        {
            if (IsValidKey(key))
            {

                /* Null value means that key marked to clear. */
                Properties[key] = null;
            }
            return this;
        }

        private CustomProperties SetObject(string key, object value)
        {
            if (IsValidKey(key))
            {
                if (value != null)
                {
                    Properties[key] = value;
                }
                else
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property value cannot be null, did you mean to call clear?");
                }
            }
            return this;
        }

        private bool IsValidKey(string key)
        {
            if (key == null || !KeyPattern.IsMatch(key))
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property \"" + key + "\" must match \"" + KeyPattern + "\"");
                return false;
            }
            if (Properties.ContainsKey(key))
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property \"" + key + "\" is already set or cleared and will be overridden.");
            }
            return true;
        }
    }
}
