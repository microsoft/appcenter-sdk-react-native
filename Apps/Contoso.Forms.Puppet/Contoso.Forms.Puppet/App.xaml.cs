using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Microsoft.AppCenter.Rum;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class App
    {
        public const string LogTag = "AppCenterXamarinPuppet";

        // App Center keys
        private const string UwpKey = "a678b499-1912-4a94-9d97-25b569284d3a";
        private const string AndroidKey = "bff0949b-7970-439d-9745-92cdc59b10fe";
        private const string IosKey = "b889c4f2-9ac2-4e2e-ae16-dae54f2c5899";

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPuppetPage());

            AppCenterLog.Assert(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenterLog.Info(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
            AppCenterLog.Info(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

            // set callbacks
            Crashes.ShouldProcessErrorReport = ShouldProcess;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
            Crashes.GetErrorAttachments = GetErrorAttachments;
            Distribute.ReleaseAvailable = OnReleaseAvailable;

            AppCenterLog.Assert(LogTag, "AppCenter.Configured=" + AppCenter.Configured);
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            Distribute.SetInstallUrl("http://install.asgard-int.trafficmanager.net");
            Distribute.SetApiUrl("https://asgard-int.trafficmanager.net/api/v0.1");
            RealUserMeasurements.SetRumKey("b1919553367d44d8b0ae72594c74e0ff");
            AppCenter.Start($"uwp={UwpKey};android={AndroidKey};ios={IosKey}",
                               typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push), typeof(RealUserMeasurements));

            // Need to use reflection because moving this to the Android specific
            // code causes crash. (Unable to access properties before init is called).
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                if (!Properties.ContainsKey(OthersContentPage.FirebaseEnabledKey))
                {
                    Properties[OthersContentPage.FirebaseEnabledKey] = false;
                }

                if ((bool)Properties[OthersContentPage.FirebaseEnabledKey])
                {
                    typeof(Push).GetRuntimeMethod("EnableFirebaseAnalytics", new Type[0]).Invoke(null, null);
                }
            }

            AppCenter.IsEnabledAsync().ContinueWith(enabled =>
            {
                AppCenterLog.Info(LogTag, "AppCenter.Enabled=" + enabled.Result);
            });
            AppCenter.GetInstallIdAsync().ContinueWith(installId =>
            {
                AppCenterLog.Info(LogTag, "AppCenter.InstallId=" + installId.Result);
            });
            AppCenterLog.Info(LogTag, "AppCenter.SdkVersion=" + AppCenter.SdkVersion);
            Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
            {
                AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.Exception=" + report.Result?.Exception);
            });
        }

        static App()
        {
            // set event handlers in static constructor to avoid duplication
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
            Push.PushNotificationReceived += PrintNotification;
        }

        static void PrintNotification(object sender, PushNotificationReceivedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                var message = e.Message;
                if (e.CustomData != null && e.CustomData.Count > 0)
                {
                    message += "\nCustom data = {" + string.Join(",", e.CustomData.Select(kv => kv.Key + "=" + kv.Value)) + "}";
                }
                Current.MainPage.DisplayAlert(e.Title, message, "OK");
            });
        }

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");

            var report = e.Report;

            //test some values
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");

            var report = e.Report;

            //test some values
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else
            {
                AppCenterLog.Info(LogTag, "No system exception was found");
            }

            if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");

            var report = e.Report;

            //test some values
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }

            if (e.Exception != null)
            {
                AppCenterLog.Info(LogTag, "There is an exception associated with the failure");
            }
        }

        bool ShouldProcess(ErrorReport report)
        {
            AppCenterLog.Info(LogTag, "Determining whether to process error report");
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
                    AppCenterLog.Debug(LogTag, "User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });

            return true;
        }

        static IEnumerable<ErrorAttachmentLog> GetErrorAttachments(ErrorReport report)
        {
            return new[]
            {
                ErrorAttachmentLog.AttachmentWithText("Hello world!", "hello.txt"),
                null,
                ErrorAttachmentLog.AttachmentWithBinary(Encoding.UTF8.GetBytes("Fake image"), "fake_image.jpeg", "image/jpeg")
            };
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            AppCenterLog.Info(LogTag, "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var title = "Version " + releaseDetails.ShortVersion + " available!";
                Task answer;
                if (releaseDetails.MandatoryUpdate)
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!");
                }
                else
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!", "Maybe tomorrow...");
                }
                answer.ContinueWith((task) =>
                {
                    if (releaseDetails.MandatoryUpdate || ((Task<bool>)task).Result)
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Update);
                    }
                    else
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                    }
                });
            }
            return custom;
        }
    }
}
