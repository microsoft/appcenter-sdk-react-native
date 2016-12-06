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
                    WaitForLabelToSay("SendingErrorReportLabel", "SendingErrorReport has occured");
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
                    WaitForLabelToSay("SentErrorReportLabel", "SentErrorReport has occured");
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
                    WaitForLabelToSay("FailedToSendErrorReportLabel", "FailedToSendErrorReport has occured");
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
                    WaitForLabelToSay("GetErrorAttachmentLabel", "GetErrorAttachment has occured");
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
                    WaitForLabelToSay("ShouldProcessErrorReportLabel", "ShouldProcessErrorReport has occured");
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
                    WaitForLabelToSay("ShouldAwaitUserConfirmationLabel", "ShouldAwaitUserConfirmation has occured");
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
