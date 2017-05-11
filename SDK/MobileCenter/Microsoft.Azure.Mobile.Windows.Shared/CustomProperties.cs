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
        private const int MaxCustomPropertiesCount = 60;
        private const int MaxCustomPropertiesKeyLength = 128;
        private const int MaxCustomPropertiesStringValueLength = 128;

        internal Dictionary<string, object> Properties { get; }

        public CustomProperties()
        {
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, string value)
        {
            if (ValidateKey(key))
            {
                if (value == null)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property value cannot be null, did you mean to call clear?");
                }
                else if (value.Length > MaxCustomPropertiesStringValueLength)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property value cannot cannot be longer than \"" + MaxCustomPropertiesStringValueLength + "\" characters");
                }
                else
                {
                    Properties[key] = value;
                }
            }
            return this;
        }

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
        public CustomProperties Set(string key, long value) => SetObject(key, value);

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
        public CustomProperties Set(string key, double value) => SetObject(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, decimal value) => SetObject(key, value);

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
            if (ValidateKey(key))
            {

                /* Null value means that key marked to clear. */
                Properties[key] = null;
            }
            return this;
        }

        private CustomProperties SetObject(string key, object value)
        {
            if (ValidateKey(key))
            {
                if (value == null)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property value cannot be null, did you mean to call clear?");
                }
                else
                {
                    Properties[key] = value;
                }
            }
            return this;
        }

        private bool ValidateKey(string key)
        {
            if (key == null || !KeyPattern.IsMatch(key))
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property \"" + key + "\" must match \"" + KeyPattern + "\"");
                return false;
            }
            if (key.Length > MaxCustomPropertiesKeyLength)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property \"" + key + "\" key length cannot be longer than \"" + MaxCustomPropertiesKeyLength + "\" characters.");
                return false;
            }
            if (Properties.ContainsKey(key))
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom property \"" + key + "\" is already set or cleared and will be overridden.");
            }
            else if (Properties.Count >= MaxCustomPropertiesCount)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom properties cannot contain more than \"" + MaxCustomPropertiesCount + "\" items.");
                return false;
            }
            return true;
        }
    }
}
