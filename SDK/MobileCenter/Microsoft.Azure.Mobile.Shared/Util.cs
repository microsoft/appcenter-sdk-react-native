using System;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Mobile
{
    internal static class Util
    {
        /* Gets the first instance of a GUID corresponding to the given platform name, or returns the string as-is if 
         * no identifier can be found. Logs a message if no identifiers can be found. */
        public static string GetKeyForPlatform(string keys, string keyIdentifier)
        {
            /* If there are no colons, then there are no named identifiers, but log a message in case the developer made a 
             * typing error. */
            if (!keys.Contains(":"))
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return keys;
            }

            /* This assumes that the key contains only lowercase letters, digits, and hyphens 
             * (and that it has at least one character) */
            string pattern = keyIdentifier + ':' + @"(\p{Nd}|\p{Ll}|-)+";
            Match match = Regex.Match(keys, pattern);
            if (match.Value == String.Empty)
            {
                throw new ArgumentException("Error parsing key for '" + keyIdentifier + "'", nameof(keyIdentifier));
            }
            return match.Value.Substring(keyIdentifier.Length + 1);
        }
    }
}
