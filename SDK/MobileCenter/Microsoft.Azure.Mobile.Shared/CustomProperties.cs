using System;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// Custom properties builder.
    /// Collects multiple properties to send in one log.
    /// </summary>
    public partial class CustomProperties
    {
        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, string value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, DateTime value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, int value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, long value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, float value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, double value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, decimal value) => PlatformSet(key, value);

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="value">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, bool value) => PlatformSet(key, value);

        /// <summary>
        /// Clear the property for the specified key.
        /// </summary>
        /// <param name="key">Key whose mapping is to be cleared.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Clear(string key) => PlatformClear(key);
    }
}
