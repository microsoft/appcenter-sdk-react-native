using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;

namespace Contoso.Android.Puppet
{
    using AlertDialog = global::Android.Support.V7.App.AlertDialog;

    [Activity(Label = "SXPuppet", Icon = "@drawable/icon", Theme = "@style/PuppetTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        const string LogTag = "AppCenterXamarinPuppet";

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Push.PushNotificationReceived -= PrintNotification;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the ViewPager and set it's PagerAdapter so that it can display items
            var viewPager = FindViewById(Resource.Id.viewpager) as ViewPager;
            viewPager.Adapter = new PagerAdapter(SupportFragmentManager, this);

            // Give the TabLayout the ViewPager
            var tabLayout = FindViewById(Resource.Id.tablayout) as TabLayout;
            tabLayout.SetupWithViewPager(viewPager);

            // App Center integration
            AppCenterLog.Assert(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenterLog.Info(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
            AppCenterLog.Info(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

            // Set event handlers
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;

            // Set callbacks
            Crashes.ShouldProcessErrorReport = ShouldProcess;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;

            Distribute.ReleaseAvailable = OnReleaseAvailable;
            AppCenterLog.Assert(LogTag, "AppCenter.Configured=" + AppCenter.Configured);
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            Distribute.SetInstallUrl("http://install.asgard-int.trafficmanager.net");
            Distribute.SetApiUrl("https://asgard-int.trafficmanager.net/api/v0.1");

            // Enable Firebase Analytics if set
            var enableAnalytics = Preferences.SharedPreferences.GetBoolean(Constants.FirebaseAnalyticsEnabledKey, false);
            if (enableAnalytics)
            {
                Push.EnableFirebaseAnalytics();
            }

            Push.PushNotificationReceived += PrintNotification;

            AppCenter.Start("bff0949b-7970-439d-9745-92cdc59b10fe", typeof(Analytics), typeof(Crashes),
                            typeof(Push), typeof(Distribute));

            AppCenter.IsEnabledAsync().ContinueWith(enabled =>
            {
                AppCenterLog.Info(LogTag, "AppCenter.Enabled=" + enabled.Result);
            });
            AppCenter.GetInstallIdAsync().ContinueWith(installId =>
            {
                AppCenterLog.Info(LogTag, "AppCenter.InstallId=" + installId.Result);
            });
            Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
            {
                AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.Exception=" + report.Result?.Exception);
                AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.Throwable=" + report.Result?.AndroidDetails?.Throwable);
            });
        }

        void PrintNotification(object sender, PushNotificationReceivedEventArgs e)
        {
            var alertDialog = new AlertDialog.Builder(this, Resource.Style.AppCompatDialogStyle);
            alertDialog.SetTitle(e.Title);
            var message = e.Message;
            if (e.CustomData != null && e.CustomData.Count > 0)
            {
                message += "\nCustom data = {" + string.Join(",", e.CustomData.Select(kv => kv.Key + "=" + kv.Value)) + "}";
            }
            alertDialog.SetMessage(message);
            alertDialog.Show();
        }

        void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");

            var args = e as SendingErrorReportEventArgs;
            ErrorReport report = args.Report;

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

        void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");

            var args = e as SentErrorReportEventArgs;
            ErrorReport report = args.Report;

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

        void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");

            var args = e as FailedToSendErrorReportEventArgs;
            ErrorReport report = args.Report;

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
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.crash_confirmation_dialog_title);
            builder.SetMessage(Resource.String.crash_confirmation_dialog_message);
            builder.SetPositiveButton(Resource.String.crash_confirmation_dialog_send_button, delegate
            {
                Crashes.NotifyUserConfirmation(UserConfirmation.Send);
            });
            builder.SetNegativeButton(Resource.String.crash_confirmation_dialog_not_send_button, delegate
            {
                Crashes.NotifyUserConfirmation(UserConfirmation.DontSend);
            });
            builder.SetNeutralButton(Resource.String.crash_confirmation_dialog_always_send_button, delegate
            {
                Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
            });
            builder.Create().Show();
            return true;
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            AppCenterLog.Info(LogTag, "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle(string.Format(GetString(Resource.String.version_x_available), releaseDetails.ShortVersion));
                builder.SetMessage(releaseDetails.ReleaseNotes);
                builder.SetPositiveButton(Microsoft.AppCenter.Distribute.Resource.String.appcenter_distribute_update_dialog_download, delegate
                {
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                });
                builder.SetCancelable(false);
                if (!releaseDetails.MandatoryUpdate)
                {
                    builder.SetNegativeButton(Microsoft.AppCenter.Distribute.Resource.String.appcenter_distribute_update_dialog_postpone, delegate
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                    });
                }
                builder.Create().Show();
            }
            return custom;
        }
    }
}