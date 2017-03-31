using System.Collections.Generic;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class App : Application
    {
        public const string LogTag = "MobileCenterXamarinPuppet";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPuppetPage());
        }

        protected override void OnStart()
        {
            MobileCenterLog.Assert(LogTag, "MobileCenter.LogLevel=" + MobileCenter.LogLevel);
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenterLog.Info(LogTag, "MobileCenter.LogLevel=" + MobileCenter.LogLevel);
            MobileCenterLog.Info(LogTag, "MobileCenter.Configured=" + MobileCenter.Configured);

            //set event handlers
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;

            //set callbacks
            Crashes.ShouldProcessErrorReport = ShouldProcess;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;

            MobileCenterLog.Assert(LogTag, "MobileCenter.Configured=" + MobileCenter.Configured);
            MobileCenterLog.Assert(LogTag, "MobileCenter.InstallId (before configure)=" + MobileCenter.InstallId);
            MobileCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Start("uwp=987b5941-4fac-4968-933e-98a7ff29237c;android=987b5941-4fac-4968-933e-98a7ff29237c;ios=fe2bf05d-f4f9-48a6-83d9-ea8033fbb644",
                               typeof(Analytics), typeof(Crashes));

            Analytics.TrackEvent("myEvent");
            Analytics.TrackEvent("myEvent2", new Dictionary<string, string> { { "someKey", "someValue" } });
            MobileCenterLog.Info(LogTag, "MobileCenter.InstallId=" + MobileCenter.InstallId);
            MobileCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + Crashes.HasCrashedInLastSession);
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
            {
                MobileCenterLog.Info(LogTag, " Crashes.LastSessionCrashReport.Exception=" + report.Result?.Exception);
            });
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            MobileCenterLog.Info(LogTag, "Sending error report");

            var args = e as SendingErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.Exception != null)
            {
                MobileCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                MobileCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            MobileCenterLog.Info(LogTag, "Sent error report");

            var args = e as SentErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.Exception != null)
            {
                MobileCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else
            {
                MobileCenterLog.Info(LogTag, "No system exception was found");
            }

            if (report.AndroidDetails != null)
            {
                MobileCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            MobileCenterLog.Info(LogTag, "Failed to send error report");

            var args = e as FailedToSendErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.Exception != null)
            {
                MobileCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                MobileCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }

            if (e.Exception != null)
            {
                MobileCenterLog.Info(LogTag, "There is an exception associated with the failure");
            }
        }


        bool ShouldProcess(ErrorReport report)
        {
            MobileCenterLog.Info(LogTag, "Determining whether to process error report");
            return true;
        }


        bool ConfirmationHandler()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null, "Send", "Always Send", "Don't Send").ContinueWith((arg) =>
                {
                    var answer = arg.Result;
                    UserConfirmation userConfirmationSelection;
                    if (answer == "Send")
                    {
                        userConfirmationSelection = UserConfirmation.Send;
                    }
                    else if (answer == "Always Send")
                    {
                        userConfirmationSelection = UserConfirmation.AlwaysSend;
                    }
                    else
                    {
                        userConfirmationSelection = UserConfirmation.DontSend;
                    }

                    MobileCenterLog.Debug(LogTag, "User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });

            return true;
        }
    }
}
