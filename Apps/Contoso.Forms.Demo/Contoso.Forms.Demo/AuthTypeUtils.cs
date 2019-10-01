// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public enum AuthType
    {
        B2C,
        AAD,
    };

    public static class AuthTypeUtils
    {
        public static AuthType GetPersistedAuthType()
        {
            if (Application.Current.Properties.TryGetValue(AuthTypeSettingKey, out object persistedAuthTypeObject))
            {
                string persistedAuthTypeString = (string)persistedAuthTypeObject;
                if (Enum.TryParse<AuthType>(persistedAuthTypeString, out var persistedAuthTypeEnum))
                {
                    return persistedAuthTypeEnum;
                }
            }

            return DefaultAuthType;
        }

        public static void SetPersistedAuthType(AuthType choice)
        {
            Application.Current.Properties[AuthTypeSettingKey] = choice.ToString();
        }

        public static IEnumerable<string> GetAuthTypeChoiceStrings()
        {
            foreach (var authTypeObject in Enum.GetValues(typeof(AuthType)))
            {
                yield return authTypeObject.ToString();
            }
        }

        public const AuthType DefaultAuthType = AuthType.B2C;
        private const string AuthTypeSettingKey = "authType";
    }
}
