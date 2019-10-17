// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using System;

namespace Microsoft.AppCenter.Windows.Shared.Utils
{
    /**
     * Utility to store and retrieve values for user identifiers.
    */
    public class UserIdContext
    {
        private static readonly object UserIdLock = new object();
        private static UserIdContext _instanceField;
        private string _userId = "";

        private static readonly object UserIdContextLock = new object();

        internal UserIdContext()
        {

        }

        /// <summary>
        /// Maximum allowed length for user identifier for App Center server.
        /// </summary>
        public static int UserIdMaxLength = 256;

        /// <summary>
        /// Event handler to subscribe to the user id update.
        /// </summary>
        public static event EventHandler<UserIdUpdatedEventArgs> UserIdUpdated;

        /// <summary>
        /// Unique instance.
        /// </summary>
        public static UserIdContext Instance
        {
            get
            {
                lock (UserIdLock)
                {
                    return _instanceField ?? (_instanceField = new UserIdContext());
                }
            }
            set
            {
                lock (UserIdLock)
                {
                    _instanceField = value;
                }
            }
        }

        /// <summary>
        /// Current user identifier.
        /// </summary>
        public string UserId
        {
            get
            {
                lock (UserIdContextLock)
                {
                    return _userId;
                }
            }
            set
            {
                lock (UserIdContextLock)
                {
                    if (_userId != value)
                    {
                        _userId = value;
                        UserIdUpdated?.Invoke(this, new UserIdUpdatedEventArgs { userId = value });
                    }
                }
            }
        }

        /// <summary>
        /// Check if userId is valid for App Center.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true if valid, false otherwise.</returns>
        public static bool CheckUserIdValidForAppCenter(String userId)
        {
            if (userId != null && userId.Length > USER_ID_APP_CENTER_MAX_LENGTH)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "userId is limited to " + USER_ID_APP_CENTER_MAX_LENGTH + " characters.");
                return false;
            }
            return true;
        }
    }
}
