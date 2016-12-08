using System;
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class CrashResultsHelper
    {

        public static IApp app;
        public static Platform platform;

        public static bool SendingErrorReportWasCalled
        {
            get
            {
                try
                {
                    WaitForLabelToSay(TestStrings.SendingErrorReportLabel, TestStrings.DidSendingErrorReportText);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool SentErrorReportWasCalled
        {
            get
            {
                try
                {
                    WaitForLabelToSay(TestStrings.SentErrorReportLabel, TestStrings.DidSentErrorReportText);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool FailedToSendErrorReportWasCalled
        {
            get
            {
                try
                {
                    WaitForLabelToSay(TestStrings.FailedToSendErrorReportLabel, TestStrings.DidFailedToSendErrorReportText);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool GetErrorAttachmentWasCalled
        {
            get
            {
                try
                {
                    WaitForLabelToSay(TestStrings.GetErrorAttachmentLabel, TestStrings.DidGetErrorAttachmentText);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool ShouldProcessErrorReportWasCalled
        {
            get
            {
                try
                {
                    WaitForLabelToSay(TestStrings.ShouldProcessErrorReportLabel, TestStrings.DidShouldProcessErrorReportText);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool ShouldAwaitUserConfirmationWasCalled
        {
            get
            {
                try
                {
                    WaitForLabelToSay(TestStrings.ShouldAwaitUserConfirmationLabel, TestStrings.DidShouldAwaitUserConfirmationText);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        static void WaitForLabelToSay(string labelName, string text)
        {
            app.WaitFor(() =>
            {
                AppResult[] results = app.Query(labelName);
                Assert.IsTrue(results.Length == 1);
                AppResult label = results[0];
                return label.Text == text;
            });
        }
    }
}
