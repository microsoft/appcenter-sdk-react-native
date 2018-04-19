using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class App
    {
        public const string LogTag = "AppCenterXamarinPuppet";

        // App Center keys
        const string UwpKey = "a678b499-1912-4a94-9d97-25b569284d3a";
        const string AndroidKey = "bff0949b-7970-439d-9745-92cdc59b10fe";
        const string IosKey = "b889c4f2-9ac2-4e2e-ae16-dae54f2c5899";

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPuppetPage());
        }

        protected override void OnStart()
        {
            if (!AppCenter.Configured)
            {
                AppCenterLog.Assert(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
                AppCenter.LogLevel = LogLevel.Verbose;
                AppCenterLog.Info(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
                AppCenterLog.Info(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

                // Set callbacks
                Crashes.ShouldProcessErrorReport = ShouldProcess;
                Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
                Crashes.GetErrorAttachments = GetErrorAttachments;
                Distribute.ReleaseAvailable = OnReleaseAvailable;

                // Event handlers
                Crashes.SendingErrorReport += SendingErrorReportHandler;
                Crashes.SentErrorReport += SentErrorReportHandler;
                Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
                Push.PushNotificationReceived += PrintNotification;

                AppCenterLog.Assert(LogTag, "AppCenter.Configured=" + AppCenter.Configured);
                AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
                Distribute.SetInstallUrl("http://install.appcenter-int.trafficmanager.net");
                Distribute.SetApiUrl("https://appcenter-int.trafficmanager.net/api/v0.1");
                AppCenter.Start($"uwp={UwpKey};android={AndroidKey};ios={IosKey}", typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
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
        }

        static void PrintNotification(object sender, PushNotificationReceivedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                var customData = e.CustomData != null
                    ? "{" + string.Join(",", e.CustomData.Select(i => i.Key + "=" + i.Value)) + "}"
                    : "<null>";
                var message = $"Message = {e.Message ?? "<null>"}\n" +
                              $"CustomData = {customData}";
                Current.MainPage.DisplayAlert(e.Title ?? "<null>", message, "OK");
            });
        }

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");

            var report = e.Report;

            // Test some values
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

            // Test some values
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

            // Test some values
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
            var attachments = new List<ErrorAttachmentLog>();
            if (Current.Properties.TryGetValue(CrashesContentPage.TextAttachmentKey, out var textAttachment) &&
                textAttachment is string text)
            {
                var attachment = ErrorAttachmentLog.AttachmentWithText(text, "hello.txt");
                attachments.Add(attachment);
            }
            if (Current.Properties.TryGetValue(CrashesContentPage.FileAttachmentKey, out var fileAttachment) &&
                fileAttachment is string file)
            {
                var filePicker = DependencyService.Get<IFilePicker>();
                if (filePicker != null)
                {
                    try
                    {
                        var result = filePicker.ReadFile(file);
                        if (result != null)
                        {
                            var attachment = ErrorAttachmentLog.AttachmentWithBinary(result.Item1, result.Item2, result.Item3);
                            attachments.Add(attachment);
                        }
                    }
                    catch (Exception e)
                    {
                        AppCenterLog.Warn(LogTag, "Couldn't read file attachment", e);
                        Current.Properties.Remove(CrashesContentPage.FileAttachmentKey);
                    }
                }
            }
            return attachments;
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
