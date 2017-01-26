using System;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Mobile
{
    public static partial class MobileCenter
    {
        /* 
         * Gets the first instance of an app secret corresponding to the given platform name, or returns the string 
         * as-is if no identifier can be found. Logs a message if no identifiers can be found.
         */
        private static string GetSecretForPlatform(string secrets, string platformIdentifier)
        {
            /* 
             * If there are no colons, then there are no named identifiers, but log a message in case the developer made 
             * a typing error.
             */
            if (!secrets.Contains("="))
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return secrets;
            }

            /* 
             * This assumes that the key contains only lowercase letters, digits, and hyphens 
             * (and that it has at least one character) 
             */
            var pattern = platformIdentifier + @"=([^;]+)";
            var match = Regex.Match(secrets, pattern);
            if (match.Value == string.Empty)
            {
                var message = "Error parsing key for '" + platformIdentifier + "'";
                throw new ArgumentException(message, nameof(platformIdentifier));
            }
            return match.Groups[1].Value;
        }
    }
}
