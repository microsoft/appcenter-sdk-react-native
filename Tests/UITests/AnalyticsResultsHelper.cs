using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class AnalyticsResultsHelper
    {
        // Network requests timeout in AppCenter SDK is 60 sec.
        private static readonly TimeSpan SentEventTimeout = TimeSpan.FromSeconds(60);

        public static IApp app;

        public static bool SendingEventWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.DidSendingEventLabel, TestStrings.DidSendingEventText);
            }
        }

        public static bool SentEventWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.DidSentEventLabel, TestStrings.DidSentEventText, SentEventTimeout);
            }
        }

        public static bool FailedToSendEventWasCalled
        {
            get
            {
                return WaitForLabelToSay(TestStrings.DidFailedToSendEventLabel, TestStrings.DidFailedToSendEventText);
            }
        }

        public static bool VerifyEventName()
        {
            return WaitForLabelToSay(TestStrings.EventNameLabel, "UITest Event");
        }

        static bool WaitForLabelToSay(string labelName, string text, TimeSpan? timeout = null)
        {
            try
            {
                app.WaitFor(() =>
                {
                    AppResult[] results = app.Query(labelName);
                    if (results.Length < 1)
                        return false;
                    AppResult label = results[0];
                    return label.Text == text;
                }, timeout: timeout);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
