// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Windows;

namespace Contoso.WPF.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string LogTag = "AppCenterDemo";

        protected override void OnStartup(StartupEventArgs e)
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
            Crashes.ShouldProcessErrorReport = ShouldProcess;

            // Event handlers.
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
            AppCenter.Start("f4e2a83d-3052-4884-8176-8b2c50277d16", typeof(Analytics), typeof(Crashes));
            Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
            {
                AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.Exception=" + task.Result?.Exception);
            });
        }

        bool ShouldProcess(ErrorReport report)
        {
            AppCenterLog.Info(LogTag, "Determining whether to process error report");
            return true;
        }

        bool ConfirmationHandler()
        {
            Current.Dispatcher.Invoke(new Action(() =>
            {
                var dialog = new UserConfirmationDialog();
                if (dialog.ShowDialog() == true)
                {
                    Crashes.NotifyUserConfirmation(dialog.ClickResult);
                }
            }));
            return true;
        }

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");
            var report = e.Report;
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
        }

        static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");
            var report = e.Report;
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else
            {
                AppCenterLog.Info(LogTag, "No system exception was found");
            }
        }

        static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");
            var report = e.Report;
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            if (e.Exception != null)
            {
                AppCenterLog.Info(LogTag, "There is an exception associated with the failure");
            }
        }
    }
}
