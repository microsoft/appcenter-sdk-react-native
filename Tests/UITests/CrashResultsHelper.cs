using System;
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class CrashResultsHelper
    {
        public static IApp app;

        public static bool SendingErrorReportWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.SendingErrorReportLabel, 
                                         TestStrings.DidSendingErrorReportText);
            }
        }

        public static bool SentErrorReportWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.SentErrorReportLabel,
                                         TestStrings.DidSentErrorReportText);
            }
        }

        public static bool FailedToSendErrorReportWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.FailedToSendErrorReportLabel, 
                                         TestStrings.DidFailedToSendErrorReportText);
            }
        }

        //public static bool GetErrorAttachmentWasCalled
        //{
        //    get
        //    {
        //        return WaitForLabelToSay(TestStrings.GetErrorAttachmentLabel, 
        //                                 TestStrings.DidGetErrorAttachmentText);
        //    }
        //}

        public static bool ShouldProcessErrorReportWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.ShouldProcessErrorReportLabel, 
                                         TestStrings.DidShouldProcessErrorReportText);
            }
        }

        public static bool ShouldAwaitUserConfirmationWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.ShouldAwaitUserConfirmationLabel, 
                                         TestStrings.DidShouldAwaitUserConfirmationText);
            }
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
