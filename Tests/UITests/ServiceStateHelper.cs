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
                app.WaitForElement(c => c.Marked("MobileCenterEnabledLabel"));
                AppResult[] results = app.Query(c => c.Marked("MobileCenterEnabledLabel"));
                Assert.IsTrue(results.Length == 1);
                return results[0].Text == "Mobile Center enabled";
            }
            set
            {
                if (value)
                {
                    app.WaitForElement(c => c.Marked("EnableMobileCenterButton"));
                    app.Tap(c => c.Marked("EnableMobileCenterButton"));
                }
                else
                {
                    app.WaitForElement(c => c.Marked("DisableMobileCenterButton"));
                    app.Tap(c => c.Marked("DisableMobileCenterButton"));
                }
            }
        }

        public static bool CrashesEnabled
        {
            get
            {
                app.WaitForElement(c => c.Marked("CrashesEnabledLabel"));
                AppResult[] results = app.Query(c => c.Marked("CrashesEnabledLabel"));
                Assert.IsTrue(results.Length == 1);
                return results[0].Text == "Crashes enabled";
            }
            set
            {
                if (value)
                {
                    app.WaitForElement(c => c.Marked("EnableCrashesButton"));
                    app.Tap(c => c.Marked("EnableCrashesButton"));
                }
                else
                {
                    app.WaitForElement(c => c.Marked("DisableCrashesButton"));
                    app.Tap(c => c.Marked("DisableCrashesButton"));
                }
            }
        }

        public static bool AnalyticsEnabled
        {
            get
            {
                app.WaitForElement(c => c.Marked("AnalyticsEnabledLabel"));
                AppResult[] results = app.Query(c => c.Marked("AnalyticsEnabledLabel"));
                Assert.IsTrue(results.Length == 1);
                return results[0].Text == "Analytics enabled";
            }
            set
            {
                if (value)
                {
                    app.WaitForElement(c => c.Marked("EnableAnalyticsButton"));
                    app.Tap(c => c.Marked("EnableAnalyticsButton"));
                }
                else
                {
                    app.WaitForElement(c => c.Marked("DisableAnalyticsButton"));
                    app.Tap(c => c.Marked("DisableAnalyticsButton"));
                }
            }
        }
    }
}
