using System;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Mobile
{
    public partial class MobileCenter
    {
        /* 
         * Gets the first instance of an app secret corresponding to the given platform name, or returns the string 
         * as-is if no identifier can be found. Logs a message if no identifiers can be found.
         */
        internal static string GetSecretForPlatform(string secrets, string platformIdentifier)
        {
            if (string.IsNullOrEmpty(secrets))
            {
                throw new MobileCenterException("App secrets string is null or empty");
            }

            /* 
             * If there are no colons, then there are no named identifiers, but log a message in case the developer made 
             * a typing error.
             */
            if (!secrets.Contains("="))
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return secrets;
            }

            var parseErrorMessage = $"Error parsing key for '{platformIdentifier}'";

            /* 
             * This assumes that the key contains only lowercase letters, digits, and hyphens 
             * (and that it has at least one character) 
             */
            var pattern = platformIdentifier + @"=([^;]+)";
            try
            {
                var match = Regex.Match(secrets, pattern);
                if (match.Value == string.Empty)
                {
                    throw new MobileCenterException(parseErrorMessage);
                }
                return match.Groups[1].Value;
            }
            catch (ArgumentException e)
            {
                throw new MobileCenterException(parseErrorMessage, e);
            }
            catch (RegexMatchTimeoutException e)
            {
                throw new MobileCenterException(parseErrorMessage, e);
            }
        }

        /// <summary>
        /// Set the custom properties.
        /// </summary>
        /// <param name="customProperties">Custom properties object.</param>
        public static void SetCustomProperties(CustomProperties customProperties)
        {
            PlatformSetCustomProperties(customProperties);
        }
    }
}
