using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Test
{
    public partial class TestPage : ContentPage
    {
        public void SendingErrorReport(object sender, SendingErrorReportEventArgs e)
        {
            
        }

        public void SentErrorReport(object sender, SentErrorReportEventArgs e)
        {
        }

        public void FailedToSendErrorReport(object sender, FailedToSendErrorReportEventArgs e)
        {
        }

        public ErrorAttachment GetErrorAttachment(ErrorReport report)
        {
            return null;
        }

        public bool ShouldProcessErrorReport(ErrorReport report)
        {
            return true;
        }

        public bool ShouldAwaitUserConfirmation()
        {
            return false;
        }

    }
}
