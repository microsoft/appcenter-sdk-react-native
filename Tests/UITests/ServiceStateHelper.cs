// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class ServiceStateHelper
    {
        public static IApp app;

        public static bool AppCenterEnabled
        {
            get
            {
                return IsPropertyEnabled(TestStrings.AppCenterEnabledLabel, TestStrings.AppCenterEnabledText);
            }
            set
            {
                ToggleProperty(TestStrings.EnableAppCenterButton, TestStrings.DisableAppCenterButton, value);
            }
        }

        public static bool CrashesEnabled
        {
            get
            {
                return IsPropertyEnabled(TestStrings.CrashesEnabledLabel, TestStrings.CrashesEnabledText);
            }
            set
            {
                ToggleProperty(TestStrings.EnableCrashesButton, TestStrings.DisableCrashesButton, value);
            }
        }

        public static bool AnalyticsEnabled
        {
            get
            {
                return IsPropertyEnabled(TestStrings.AnalyticsEnabledLabel, TestStrings.AnalyticsEnabledText);
            }
            set
            {
                ToggleProperty(TestStrings.EnableAnalyticsButton, TestStrings.DisableAnalyticsButton, value);
            }
        }

        static bool IsPropertyEnabled(string elementId, string enabledText)
        {
            app.WaitForElement(elementId);
            AppResult[] results = app.Query(elementId);
            Assert.IsTrue(results.Length == 1);
            return results[0].Text == enabledText;
        }

        static void ToggleProperty(string enableElementId, string disableElementId, bool shouldEnable)
        {
            if (shouldEnable)
            {
                app.WaitForElement(enableElementId);
                app.Tap(enableElementId);
            }
            else
            {
                app.WaitForElement(disableElementId);
                app.Tap(disableElementId);
            }
        }
    }
}
