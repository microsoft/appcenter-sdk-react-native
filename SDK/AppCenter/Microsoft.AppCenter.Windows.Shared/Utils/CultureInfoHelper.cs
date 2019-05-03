// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AppCenter.Windows.Shared.Utils
{
    // https://www.pedrolamas.com/2017/05/03/cultureinfo-changes-in-uwp-part-2/
    class CultureInfoHelper
    {
        // Relative location of system locale name.
        private const uint LOCALE_SNAME = 0x0000005c;

        // Constants used by the National Language Support reference.
        private const string LOCALE_NAME_USER_DEFAULT = null;
        private const string LOCALE_NAME_SYSTEM_DEFAULT = "!x-sys-default-locale";

        private const int BUFFER_SIZE = 530;

        [DllImport("api-ms-win-core-localization-l1-2-0.dll", CharSet = CharSet.Unicode)]
        private static extern int GetLocaleInfoEx(string lpLocaleName, uint LCType, StringBuilder lpLCData, int cchData);

        public static CultureInfo GetCurrentCulture()
        {
            var name = InvokeGetLocaleInfoEx(LOCALE_NAME_USER_DEFAULT, LOCALE_SNAME);
            if (name == null)
            {
                name = InvokeGetLocaleInfoEx(LOCALE_NAME_SYSTEM_DEFAULT, LOCALE_SNAME);
                if (name == null)
                {
                    // If system/user default doesn't work, use current culture which is app locale.
                    return CultureInfo.CurrentCulture;
                }
            }
            return new CultureInfo(name);
        }

        private static string InvokeGetLocaleInfoEx(string lpLocaleName, uint LCType)
        {
            try
            {
                var buffer = new StringBuilder(BUFFER_SIZE);
                var resultCode = GetLocaleInfoEx(lpLocaleName, LCType, buffer, BUFFER_SIZE);
                if (resultCode > 0)
                {
                    return buffer.ToString();
                }
            }
            catch (DllNotFoundException exception)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Failed to call GetLocaleInfoEx: {exception.Message}");
            }
            return null;
        }
    }
}
