// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Auth
{
    /// <summary>
    /// Result for sign-in operation. 
    /// </summary>
    public class SignInResult
    {
        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <value>The user information or null if an error occurred.</value>
        public UserInformation UserInformation { get; internal set; }

        /// <summary>
        /// Error that occurred during sign-in.
        /// </summary>
        /// <value>The exception that describes the error or null if sign-in succeeded.</value>
        public Exception Exception { get; internal set; }
    }
}
