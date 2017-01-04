using System;
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class LastSessionErrorReportHelper
    {
        public static IApp app;

        public static bool DeviceReported
        {
            get
            {
                return WaitForLabelToSay(TestStrings.DeviceLabel,
                                         TestStrings.DeviceReportedText);
            }
        }

        public static bool HasIosDetails
        {
            get
            {
                return WaitForLabelToSay(TestStrings.iOSDetailsLabel,
                                         TestStrings.HasiOSDetailsText);
            }
        }

        public static bool HasAndroidDetails
        {
            get
            {
                return WaitForLabelToSay(TestStrings.AndroidDetailsLabel,
                                         TestStrings.HasAndroidDetailsText);
            }
        }

        public static bool VerifyExceptionType(string expectedType)
        {
            return WaitForLabelToSay(TestStrings.ExceptionTypeLabel, expectedType);
        }

        public static bool VerifyExceptionMessage(string expectedMessage)
        {
            return WaitForLabelToSay(TestStrings.ExceptionMessageLabel, expectedMessage);
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
