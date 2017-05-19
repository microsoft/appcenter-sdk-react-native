using System;

namespace Microsoft.Azure.Mobile
{
    public partial class MobileCenter
    {
        // Gets the first instance of an app secret corresponding to the given platform name, or returns the string 
        // as-is if no identifier can be found. Logs a message if no identifiers can be found.
        internal static string GetSecretForPlatform(string secrets, string platformIdentifier)
        {
            if (string.IsNullOrEmpty(secrets))
            {
                throw new MobileCenterException("App secrets string is null or empty");
            }

            // If there are no equals signs, then there are no named identifiers, but log a message in case the developer made 
            // a typing error.
            if (!secrets.Contains("="))
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return secrets;
            }

            var parseErrorMessage = $"Error parsing key for '{platformIdentifier}'";

            var platformIndicator = platformIdentifier + "=";
            var secretIdx = secrets.IndexOf(platformIndicator, StringComparison.Ordinal) + platformIndicator.Length;
            if (secretIdx == -1)
            {
                throw new MobileCenterException(parseErrorMessage);
            }

            var platformSecret = string.Empty;

            while (secretIdx < secrets.Length)
            {
                var nextChar = secrets[secretIdx++];
                if (nextChar == ';')
                {
                    break;
                }

                platformSecret += nextChar;
            }

            if (platformSecret == string.Empty)
            {
                throw new MobileCenterException(parseErrorMessage);
            }

            return platformSecret;
        }
    }
}
