// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Microsoft.AppCenter.Crashes
{
    internal static class CrashesUtils
    {
        internal static byte[] SerializeException(Exception exception)
        {
            return Encoding.UTF8.GetBytes(exception.ToString());
        }

        internal static string DeserializeException(byte[] exceptionBytes)
        {
            try
            {
                return Encoding.UTF8.GetString(exceptionBytes);
            }
            catch (Exception e)
            {
                AppCenterLog.Warn(Crashes.LogTag, "Failed to deserialize exception for client side inspection.", e);
            }
            return null;
        }
    }
}
