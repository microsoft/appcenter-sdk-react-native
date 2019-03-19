// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.Forms.Test
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            TestPage testPage = new TestPage();

            /* Set crash callbacks and events */
            Crashes.ShouldProcessErrorReport = testPage.ResultsPage.ShouldProcessErrorReport;
            Crashes.ShouldAwaitUserConfirmation = testPage.ResultsPage.ShouldAwaitUserConfirmation;
            Crashes.SendingErrorReport += testPage.ResultsPage.SendingErrorReport;
            Crashes.SentErrorReport += testPage.ResultsPage.SentErrorReport;
            Crashes.FailedToSendErrorReport += testPage.ResultsPage.FailedToSendErrorReport;

            /* Start App Center */
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(typeof(Analytics), typeof(Crashes));
            MainPage = testPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
