// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Auth
{
    /// <summary>
    /// User information.
    /// </summary>
    public class UserInformation
    {
        /// <summary>
        /// Account identifier.
        /// </summary>
        /// <value>The account identifier.</value>
        public string AccountId { get; private set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the identifier token.
        /// </summary>
        /// <value>The identifier token.</value>
        public string IdToken { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.UserInformation"/> class.
        /// </summary>
        /// <param name="accountId">Account identifier.</param>
        /// <param name="accessToken">Access token.</param>
        /// <param name="idToken">Identifier token.</param>
        internal UserInformation(string accountId, string accessToken, string idToken)
        {
            AccountId = accountId;
            AccessToken = accessToken;
            IdToken = idToken;
        }
    }
}
