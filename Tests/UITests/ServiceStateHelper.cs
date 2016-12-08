using System;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class ServiceStateHelper
    {
        public static IApp app;
        public static Platform platform;

        public static bool MobileCenterEnabled
        {
            get
            {
                app.WaitForElement(TestStrings.MobileCenterEnabledLabel);
                AppResult[] results = app.Query(TestStrings.MobileCenterEnabledLabel);
                Assert.IsTrue(results.Length == 1);
                return results[0].Text == TestStrings.MobileCenterEnabledText;
            }
            set
            {
                if (value)
                {
                    app.WaitForElement(TestStrings.EnableMobileCenterButton);
                    app.Tap(TestStrings.EnableMobileCenterButton);
                }
                else
                {
                    app.WaitForElement(TestStrings.DisableMobileCenterButton);
                    app.Tap(TestStrings.DisableMobileCenterButton);
                }
            }
        }

        public static bool CrashesEnabled
        {
            get
            {
                app.WaitForElement(TestStrings.CrashesEnabledLabel);
                AppResult[] results = app.Query(TestStrings.CrashesEnabledLabel);
                Assert.IsTrue(results.Length == 1);
                return results[0].Text == TestStrings.CrashesEnabledText;
            }
            set
            {
                if (value)
                {
                    app.WaitForElement(TestStrings.EnableCrashesButton);
                    app.Tap(TestStrings.EnableCrashesButton);
                }
                else
                {
                    app.WaitForElement(TestStrings.DisableCrashesButton);
                    app.Tap(TestStrings.DisableCrashesButton);
                }
            }
        }

        public static bool AnalyticsEnabled
        {
            get
            {
                app.WaitForElement(TestStrings.AnalyticsEnabledLabel);
                AppResult[] results = app.Query(TestStrings.AnalyticsEnabledLabel);
                Assert.IsTrue(results.Length == 1);
                return results[0].Text == TestStrings.AnalyticsEnabledText;
            }
            set
            {
                if (value)
                {
                    app.WaitForElement(TestStrings.EnableAnalyticsButton);
                    app.Tap(TestStrings.EnableAnalyticsButton);
                }
                else
                {
                    app.WaitForElement(TestStrings.DisableAnalyticsButton);
                    app.Tap(TestStrings.DisableAnalyticsButton);
                }
            }
        }
    }
}
