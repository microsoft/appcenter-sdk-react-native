using System;
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class AnalyticsResultsHelper
    {
        public static IApp app;

        public static bool VerifyNumProperties(int count)
        {
            return WaitForLabelToSay(TestStrings.EventPropertiesLabel, count.ToString());
        }

        public static bool VerifyEventName()
        {
            return WaitForLabelToSay(TestStrings.EventNameLabel, "UITest Event");
        }

        static bool WaitForLabelToSay(string labelName, string text)
        {
            try
            {
                app.WaitFor(() =>
                {
                    AppResult[] results = app.Query(labelName);
                    Assert.IsTrue(results.Length == 1);
                    AppResult label = results[0];
                    return label.Text == text;
                });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
