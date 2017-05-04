using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;

namespace Contoso.Android.Puppet
{
    using AlertDialog = global::Android.Support.V7.App.AlertDialog;

    [Activity(Label = "SXPuppet", Icon = "@drawable/icon", Theme = "@style/PuppetTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        const string LogTag = "MobileCenterXamarinPuppet";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the ViewPager and set it's PagerAdapter so that it can display items
            var viewPager = FindViewById(Resource.Id.viewpager) as ViewPager;
            viewPager.Adapter = new PagerAdapter(SupportFragmentManager, this);
            viewPager.PageSelected += delegate {
                viewPager.Adapter.NotifyDataSetChanged();
            };

            // Give the TabLayout the ViewPager
            var tabLayout = FindViewById(Resource.Id.tablayout) as TabLayout;
            tabLayout.SetupWithViewPager(viewPager);

            // Mobile Center integration
            MobileCenterLog.Assert(LogTag, "MobileCenter.LogLevel=" + MobileCenter.LogLevel);
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenterLog.Info(LogTag, "MobileCenter.LogLevel=" + MobileCenter.LogLevel);
            MobileCenterLog.Info(LogTag, "MobileCenter.Configured=" + MobileCenter.Configured);

            // Set event handlers
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;

            // Set callbacks
            Crashes.ShouldProcessErrorReport = ShouldProcess;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;

            Distribute.ReleaseAvailable = OnReleaseAvailable;

            MobileCenterLog.Assert(LogTag, "MobileCenter.Configured=" + MobileCenter.Configured);
            MobileCenterLog.Assert(LogTag, "MobileCenter.InstallId (before configure)=" + MobileCenter.InstallId);
            MobileCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            Distribute.SetInstallUrl("http://install.asgard-int.trafficmanager.net");
            Distribute.SetApiUrl("https://asgard-int.trafficmanager.net/api/v0.1");
            MobileCenter.Start("bff0949b-7970-439d-9745-92cdc59b10fe", typeof(Analytics), typeof(Crashes), typeof(Distribute));

            MobileCenterLog.Info(LogTag, "MobileCenter.InstallId=" + MobileCenter.InstallId);
            MobileCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + Crashes.HasCrashedInLastSession);
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
            {
                MobileCenterLog.Info(LogTag, " Crashes.LastSessionCrashReport.Exception=" + report.Result?.Exception);
            });
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
            MobileCenterLog.Info(LogTag, "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle(string.Format(GetString(Resource.String.version_x_available), releaseDetails.ShortVersion));
                builder.SetMessage(releaseDetails.ReleaseNotes);
                builder.SetPositiveButton(Microsoft.Azure.Mobile.Distribute.Resource.String.mobile_center_distribute_update_dialog_download, delegate
                {
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                });
                builder.SetCancelable(false);
                if (!releaseDetails.MandatoryUpdate)
                {
                    builder.SetNegativeButton(Microsoft.Azure.Mobile.Distribute.Resource.String.mobile_center_distribute_update_dialog_postpone, delegate
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